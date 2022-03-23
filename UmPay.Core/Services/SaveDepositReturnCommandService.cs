using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services;

internal class SaveDepositReturnCommandService : ICommandService<SaveDepositReturnCommand>
{
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly UmPaySerializer _umPaySerializer;

    public SaveDepositReturnCommandService(
        IPaymentServiceClient paymentServiceClient,
        UmPaySerializer umPaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));
    }

    public ValueTask ExecuteAsync(
        SaveDepositReturnCommand command,
        CancellationToken cancellationToken = default)
        => _paymentServiceClient.SubmitDepositFormStatusAsync(
            command.FormId,
            command.State,
            _umPaySerializer.Serialize(command.DepositReturn));
}
