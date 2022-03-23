using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay;

public class UmPayDepositResponse
{
    /// <summary>
    /// 請求狀態碼
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    /// <summary>
    /// 狀態碼說明
    /// </summary>
    [JsonPropertyName("msg")]
    public string Msg { get; set; } = default!;

    /// <summary>
    /// 商戶訂單號
    /// </summary>
    [JsonPropertyName("outTradeNo")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// KG平台訂單號
    /// </summary>
    [JsonPropertyName("tradeNo")]
    public string? TradeNo { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    [JsonPropertyName("tradeStatus")]
    public int? TradeStatus { get; set; }

    /// <summary>
    /// 訂單金額
    /// </summary>
    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    /// <summary>
    /// 跳轉表單html
    /// </summary>
    [JsonPropertyName("jumpUrl")]
    public string? JumpUrl { get; set; }
}
