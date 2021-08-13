using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay.ViewModels
{
    public class CallbackDepositViewModel
    {
        [JsonPropertyName("merchantNo")]
        public string MerchantNo { get; set; }

        [JsonPropertyName("tradeType")]
        public string TradeType { get; set; }

        [JsonPropertyName("orderType")]
        public string OrderType { get; set; }

        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; set; }

        [JsonPropertyName("orderNo")]
        public string OrderNo { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("tradeDesc")]
        public string TradeDesc { get; set; }

        [JsonPropertyName("notifyUrl")]
        public string NotifyUrl { get; set; }

        [JsonPropertyName("sign")]
        public string Sign { get; set; }
    }
}