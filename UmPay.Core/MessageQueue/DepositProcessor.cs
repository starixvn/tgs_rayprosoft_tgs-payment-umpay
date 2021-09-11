using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.UmPay.MessageQueue
{
    internal class DepositProcessor : IMessageProcessor
    {
        private readonly UmPaySerializer _umPaySerializer;
        private readonly UmPayClient _umPayClient;
        private readonly UmPayOptions _options;

        public DepositProcessor(
            UmPaySerializer umPaySerializer,
            UmPayClient umPayClient,
            IOptions<UmPayOptions> optionsAccessor)
        {
            _umPaySerializer = umPaySerializer ?? throw new ArgumentNullException(nameof(umPaySerializer));
            _umPayClient = umPayClient ?? throw new ArgumentNullException(nameof(umPayClient));
            _options = optionsAccessor.Value;
        }

        public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var request = DepositRequest.Parser.ParseFrom(data);
            var settings = _umPaySerializer.Deserialize<UmPayDepositSettings>(request.ApplyInfo)!;
            var cryptoTradeTypes = new string[] { "PAY10", "PAY11", "PAY12" };
            var randomPointTradeTypes = new string[] { "PAY01", "PAY03", "PAY04" };

            var amount = decimal.Parse(request.Amount);
            if (randomPointTradeTypes.Contains(settings.TradeType))
            {
                amount = decimal.Parse((request.Amount + "." + new Random().Next(0, 99).ToString()));
            }

            var order = new DepositOrder
            {
                MerchatNo = settings.MerchantNo,
                TradeType = settings.TradeType,
                OrderType = settings.OrderType,
                //不影響用戶付款時間
                TimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString(),
                //訂單號需純數字
                OrderNo = new BigInteger(settings.OrderNo.ToByteArray(), isUnsigned: true, isBigEndian: false).ToString(),
                Amount = amount,
                Rate = cryptoTradeTypes.Contains(settings.TradeType) ? settings.Rate : 1,
                TradeDesc = settings.TradeDesc,
                NotifyUrl = $"{_options.CallbackBaseUrl}/deposit/callback"
            };

            var depositResponse = await _umPayClient
                .DepositAsync(settings.MerchantNo, settings.ApiKey, order)
                .ConfigureAwait(false);

            if (depositResponse != null && depositResponse.Code == "1")
            {
                var applyInfo = new UmPayDepositApplyInfo(settings, depositResponse);

                var msg = new DepositResponse
                {
                    Result = _umPaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
            else
            {
                var errorMsg = string.Empty;
                if (depositResponse != null)
                {
                    if (depositResponse.Msg != null)
                        errorMsg = depositResponse.Msg;
                    else
                        errorMsg = "Request Fail";
                }

                var response = new UmPayDepositFailResponse
                {
                    Code = depositResponse!.Code,
                    Msg = errorMsg
                };

                var applyInfo = new UmPayDepositApplyInfo(settings, response);

                var msg = new DepositResponse
                {
                    Result = _umPaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}