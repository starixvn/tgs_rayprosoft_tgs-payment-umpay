using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay;

public class CancelDepositOrder
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
    [JsonPropertyName("orderNo")]
    public string OrderNo { get; init; } = default!;

    [JsonPropertyName("sign")]
    public string? Sign { get; set; }
}
