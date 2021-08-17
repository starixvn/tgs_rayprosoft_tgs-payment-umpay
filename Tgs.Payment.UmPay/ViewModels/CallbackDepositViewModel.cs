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
        public int TradeStatus { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("real_amount")]
        public decimal RealAmount { get; set; }

        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = default!;

        //[JsonPropertyName("orderNo")]
        //public string? OrderNo { get; set; }

        //[JsonPropertyName("notifyUrl")]
        //public string? NotifyUrl { get; set; }
        //[JsonPropertyName("orderType")]
        //public string? OrderType { get; set; }
        //[JsonPropertyName("timeStamp")]
        //public string? TimeStamp { get; set; }

        //[JsonPropertyName("tradeDesc")]
        //public string? TradeDesc { get; set; }
        //[JsonPropertyName("tradeType")]
        //public string? TradeType { get; set; }

        //[JsonPropertyName("rate")]
        //public decimal Rate { get; set; } = default!;
    }
}