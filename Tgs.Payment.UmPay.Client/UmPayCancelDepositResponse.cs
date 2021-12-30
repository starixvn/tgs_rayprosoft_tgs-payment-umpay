using System.Text.Json.Serialization;

namespace Tgs.Payment.UmPay
{
    public class UmPayCancelDepositResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = default!;
    }
}