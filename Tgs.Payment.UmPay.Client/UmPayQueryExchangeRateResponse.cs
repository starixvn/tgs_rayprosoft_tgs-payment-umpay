using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay
{
    public class UmPayQueryExchangeRateResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("rate")]
        public JsonDocument Rate { get; set; }
    }
}