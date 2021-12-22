using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;

namespace Tgs.Payment.UmPay.MessageQueue
{
    internal class DepositRejectSender
    {
        private readonly IPaymentClient _client;
        private readonly ILogger<DepositRejectSender> _logger;

        public DepositRejectSender(
            IPaymentClient client,
            ILogger<DepositRejectSender> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask SendRejectDepositAsync(Guid formId, Guid brandId)
        {
            _logger.LogInformation($"Send Reject Deposit request: form Id:{formId}, brand Id:{brandId}");

            await _client.RejectDepositAsync(formId, brandId);
        }
    }
}