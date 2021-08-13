using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.Payment.UmPay;


namespace UmPayTest
{
    internal class Demo : BackgroundService
    {
        private readonly IPaymentClient _paymentClient;
        private readonly UmPaySerializer _umPaySerializer;
        private readonly ILogger<Demo> _logger;

        public Demo(IPaymentClient paymentClient, UmPaySerializer umPaySerializer, ILogger<Demo> logger)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var apiKey = "7bdc9a41b4d54ca1f593cddf00f49023";
            var merchantNo = "MAJA1628579663";
            var orderType = "1";
            var orderNo = Guid.NewGuid();
            var amount = 100;
            var tradeType = "PAY01";
            var cryptoTradeTypes = new string[] { "PAY10", "PAY11", "PAY12" };
            var rate = cryptoTradeTypes.Contains(tradeType) ? 7.8m : 1;//TODO:加密需要給rate
            var tradeDesc = "order123";
            var depositSettings = new UmPayDepositSettings(
                apiKey,
                merchantNo,
                tradeType,
                orderType,
                orderNo,
                amount,
                rate,
                tradeDesc
                );
            try
            {
                var depositReturn = await _paymentClient.DepositAsync(
                    "tgs.payment.umpay.deposit",
                    100,
                    _umPaySerializer.Serialize(depositSettings));
                _logger.LogInformation($"Deposit Return:", depositReturn);
            }
            catch(Exception ex)
            {
                _logger.LogDebug(ex.Message);
            }
        }
    }
}
