using System;
using System.Text.Json;

namespace Tgs.Payment.UmPay
{
    public class UmPayOptions
    {
        public string CallbackBaseUrl { get; set; } = default!;
        public string NatsUrl { get; set; } = default!;

        public Uri PaymentApiUrl { get; set; } = default!;

        public Uri PaymentServiceBaseUrl { get; set; } = default!;
        public JsonSerializerOptions? SystemSerializerOptions { get; set; }
    }
}