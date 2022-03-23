using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services;

internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormInfo?>
{
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly UmPaySerializer _umPaySerializer;
    private readonly ILogger<GetDepositFormQueryService> _logger;

    public GetDepositFormQueryService(
        IPaymentServiceClient paymentServiceClient,
        UmPaySerializer umPaySerializer,
        ILogger<GetDepositFormQueryService> logger)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<DepositFormInfo?> GetAsync(GetDepositFormQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Before Parse orderNo: {query.OrderNo}", query.OrderNo);
        var depositFormId = BigIntegerStringToGuid(query.OrderNo);
        _logger.LogInformation("After Parse depositFormId: {depositFormId}", depositFormId);

        var formData = await _paymentServiceClient
        .GetDepositFormAsync(depositFormId)
        .ConfigureAwait(false);

        if (formData == null)
            return null;

        var applyInfo = string.IsNullOrEmpty(formData.ApplyInfo)
            ? null
            : _umPaySerializer.Deserialize<UmPayDepositApplyInfo>(formData.ApplyInfo);

        return formData == null
            ? null
            : new DepositFormInfo(
                formData.Id,
                formData.PlayerCode,
                formData.BrandId,
                formData.BrandPaymentId,
                formData.Amount,
                formData.Status,
                applyInfo,
                formData.PaymentReturn);
    }

    private static Guid BigIntegerStringToGuid(string orderNo)
    {
        var bigInt = BigInteger.Parse(orderNo, NumberStyles.None);
        var buffer = new Span<byte>(new byte[16]);
        bigInt.ToByteArray().Take(16).ToArray().CopyTo(buffer);

        return new Guid(buffer);
    }
}
