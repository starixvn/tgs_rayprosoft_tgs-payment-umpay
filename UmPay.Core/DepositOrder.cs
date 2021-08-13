using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay
{
    public class DepositOrder
    {
        /// <summary>
        /// 商戶號
        /// </summary>
        [JsonPropertyName("merchantNo")]
        public string MerchatNo { get; init; } = default!;
        /// <summary>
        /// 通道類型:
        /// PAY01: 支轉支
        /// PAY02:便利店充現金
        /// PAY03:轉數快
        /// PAY04:網銀
        /// PAY05:入鈔機
        /// PAY06:微信支付(不支援)
        /// PAY10:USDT - ERC20
        /// PAY11: USDT - TRC20
        /// PAY12: USDT - OMNI
        /// PAY13: EHK(正式環境未開放)
        /// </summary>
        [JsonPropertyName("tradeType")]
        public string TradeType { get; init; } = default!;
        /// <summary>
        /// 下單類型 預設1=正常 2=重新下單
        /// </summary>

        [JsonPropertyName("orderType")]
        public string OrderType { get; init; } = default!;
        /// <summary>
        /// 10位數時間戳
        /// </summary>
        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; init; } = default!;
        /// <summary>
        /// 商戶訂單號-請帶純數字
        /// </summary>
        [JsonPropertyName("orderNo")]
        public string OrderNo { get; init; } = default!;
        /// <summary>
        /// 交易金額
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// 匯率
        /// 這三種通道類型 需給匯率
        /// PAY10:USDT - ERC20
        /// PAY11: USDT - TRC20
        /// PAY12: USDT - OMNI
        /// </summary>
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// 交易描述
        /// </summary>

        [JsonPropertyName("tradeDesc")]       
        public string TradeDesc { get; init; } = default!;
        /// <summary>
        /// 異步通知地址
        /// </summary>

        [JsonPropertyName("notifyUrl")]
        public string NotifyUrl { get; set; } = default!;

        [JsonPropertyName("sign")]
        public string? Sign { get; set; }
    }
}