using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay;

public class UmPayQueryExchangeRateResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    [JsonPropertyName("msg")]
    public string Msg { get; set; } = default!;

    [JsonPropertyName("rate")]
    public JsonDocument? Rate { get; set; }
}
