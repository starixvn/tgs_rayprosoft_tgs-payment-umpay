using System.Text.Json;

namespace Tgs.Payment.UmPay
{
    public record UmPayQueryExchangeRateResponse(string Code, string Msg, JsonDocument Rate);
}