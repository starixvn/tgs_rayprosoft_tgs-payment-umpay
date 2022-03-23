using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace Tgs.Payment.UmPay.MessageQueue;

internal class DepositScheduleHandler : IMessageHandler
{
    private readonly ILogger<DepositScheduleHandler> _logger;
    private readonly IPaymentServiceClient _client;
    private readonly DepositRejectSender _rejectSender;
    private readonly UmPayClient _umPayClient;
    private readonly UmPaySerializer _umPaySerializer;

    public DepositScheduleHandler(
        ILogger<DepositScheduleHandler> logger,
        IPaymentServiceClient client,
        DepositRejectSender rejectSender,
        UmPayClient umPayClient,
        UmPaySerializer umPaySerializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _rejectSender = rejectSender ?? throw new ArgumentNullException(nameof(rejectSender));
        _umPayClient = umPayClient ?? throw new ArgumentNullException(nameof(umPayClient));
        _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));
    }

    public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = ScheduleRequest.Parser.ParseFrom(data);
        _logger.LogInformation(
            "Deposit Schedule Handler Receive request Form Id:{request.FormId.ToGuid()}, Schedule Time:{request.ScheduleTime.ToDateTime():yyyy/MM/dd HH:mm:ss}",
            request.FormId.ToGuid(),
            request.ScheduleTime.ToDateTime());

        var formId = request.FormId.ToGuid();

        var depositForm = await _client
            .GetDepositFormAsync(formId)
            .ConfigureAwait(false);

        var brandId = depositForm!.BrandId;

        _logger.LogInformation(
            "GetDepositFormAsync Form Id:{depositForm.Id}, Brand Id:{brandId}, Status:{depositForm.Status}",
            depositForm.Id,
            brandId,
            depositForm.Status);

        if (depositForm.Status is PaymentFlowState.Approved
            or PaymentFlowState.Reject)
        {
            return;
        }

        var applyInfo = _umPaySerializer.Deserialize<UmPayDepositApplyInfo>(depositForm.ApplyInfo!)!;

        var cancelOrder = new CancelDepositOrder
        {
            MerchatNo = applyInfo.Settings.MerchantNo,
            OrderNo = applyInfo.Response.OutTradeNo!
        };

        _logger.LogInformation("Send Cancel Order Api To UmPay");

        var cancelDepositResponse = await _umPayClient
            .CancelDepositAsync(applyInfo.Settings.ApiKey, cancelOrder)
            .ConfigureAwait(false);

        if (cancelDepositResponse != null && cancelDepositResponse.Code == "1")
        {
            _logger.LogInformation("Send Reject Deposit request");
            await _rejectSender
                .SendRejectDepositAsync(request.FormId, brandId)
                .ConfigureAwait(false);
        }
    }
}
