using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.UmPay.MessageQueue
{
    internal class QueryExchangeRateProcessor : IMessageProcessor
    {
        private readonly UmPayOptions _options;
        private UmPayClient _payClient;
        private readonly UmPaySerializer _umPaySerializer;
        private readonly ILogger<QueryExchangeRateProcessor> _logger;

        public QueryExchangeRateProcessor(
            IOptions<UmPayOptions> optionsAccessor,
            UmPayClient payClient,
            UmPaySerializer umPaySerializer,
            ILogger<QueryExchangeRateProcessor> logger)
        {
            _options = optionsAccessor.Value;
            _payClient = payClient ?? throw new ArgumentNullException(nameof(payClient));
            _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var request = GeneralRequest.Parser.ParseFrom(data);
            var settings = _umPaySerializer.Deserialize<UmPayQueryExchangeRateSettings>(request.Content)!;

            var result = await _payClient
                .QueryExchangeRateAsync()
                .ConfigureAwait(false);

            if (result != null && result.Code == "1")
            {
                var response = new UmPayQueryExchangeRateResponse()
                {
                    Code = result.Code,
                    Msg = result.Msg,
                    Rate = result.Rate
                };

                var applyInfo = new UmPayQueryExchangeRateApplyInfo(settings, response);
                var msg = new GeneralResponse()
                {
                    Result = _umPaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
            else
            {
                var response = new UmPayQueryExchangeRateResponse();

                var applyInfo = new UmPayQueryExchangeRateApplyInfo(settings, response);

                var msg = new GeneralResponse()
                {
                    Result = _umPaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}