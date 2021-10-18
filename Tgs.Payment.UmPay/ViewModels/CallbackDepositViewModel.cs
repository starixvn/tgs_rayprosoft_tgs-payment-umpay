using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay.ViewModels
{
    public class CallbackDepositViewModel
    {
        [JsonPropertyName("outTradeNo")]
        public string OutTradeNo { get; set; } = default!;

        [JsonPropertyName("tradeNo")]
        public string TradeNo { get; set; } = default!;

        [JsonPropertyName("merchantNo")]
        public string MerchantNo { get; set; } = default!;

        [JsonPropertyName("tradeStatus")]
        public string TradeStatus { get; set; } = default!;

        [JsonPropertyName("amount")]
        public string Amount { get; set; } = default!;

        [JsonPropertyName("real_amount")]
        public string RealAmount { get; set; } = default!;

        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = default!;
    }
}