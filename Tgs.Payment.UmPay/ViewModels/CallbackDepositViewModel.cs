using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay.ViewModels
{
    public class CallbackDepositViewModel
    {
        [JsonPropertyName("merchantNo")]
        public string MerchantNo { get; set; } = default!;

        [JsonPropertyName("tradeType")]
        public string TradeType { get; set; } = default!;

        [JsonPropertyName("orderType")]
        public string OrderType { get; set; } = default!;

        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; set; } = default!;

        [JsonPropertyName("orderNo")]
        public string OrderNo { get; set; } = default!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } = default!;

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; } = default!;

        [JsonPropertyName("tradeDesc")]
        public string TradeDesc { get; set; } = default!;

        [JsonPropertyName("notifyUrl")]
        public string NotifyUrl { get; set; } = default!;

        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;
    }
}