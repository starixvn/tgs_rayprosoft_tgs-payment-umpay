using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tgs.Payment.UmPay;
using Tgs.Payment.UmPay.MessageQueue;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUmPay(this IServiceCollection services, Action<UmPayOptions, IServiceProvider> configure)
            => services
                .AddOptions<UmPayOptions>()
                .Configure(configure)
                .Services
                .AddHttpClient<UmPayClient>()
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<UmPayOptions>>().Value;

                    httpClient.BaseAddress = options.PaymentApiUrl;
                })
                .Services
                .AddPaymentServiceClient((options, sp) =>
                {
                    var umPayOptions = sp.GetRequiredService<IOptions<UmPayOptions>>().Value;

                    options.PaymentServiceBaseUrl = umPayOptions.PaymentServiceBaseUrl;

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    };
                })
                .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
                .AddSingleton<UmPayHasher>()
                .AddSingleton<UmPaySerializer>()
                .AddNatsMessageQueue(config =>
                {
                    config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var umPayOptions = sp.GetRequiredService<IOptions<UmPayOptions>>().Value;

                            options.Url = umPayOptions.NatsUrl;
                        })
                        .AddProcessor<DepositProcessor>("tgs.payment.umpay.deposit", "tgs.payment.umpay.deposit")
                        .AddProcessor<QueryExchangeRateProcessor>("tgs.payment.umpay.queryexchangerate");
                })
                .AddPaymentClient();
    }
}