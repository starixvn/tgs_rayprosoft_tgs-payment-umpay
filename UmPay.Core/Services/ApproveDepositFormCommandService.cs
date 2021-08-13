using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services
{
    internal class ApproveDepositFormCommandService : ICommandService<ApproveDepositFormCommand>
    {
        private readonly IPaymentClient _paymentClient;

        public ApproveDepositFormCommandService(IPaymentClient paymentClient)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        }

        public ValueTask ExecuteAsync(ApproveDepositFormCommand command, CancellationToken cancellationToken = default)
            => _paymentClient.ApproveDepositAsync(command.FormId, command.BrandId);
    }
}