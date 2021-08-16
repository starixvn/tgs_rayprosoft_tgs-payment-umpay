using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.UmPay
{
    public class UmPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly UmPayHasher _umPayHasher;
        private readonly ILogger<UmPayClient> _logger;

        public UmPayClient(
            HttpClient httpClient,
            IOptions<UmPayOptions> optionsAccessor,
            UmPayHasher umPayHasher,
            ILogger<UmPayClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonSerializerOptions = optionsAccessor.Value.SystemSerializerOptions;
            _umPayHasher = umPayHasher ?? throw new ArgumentNullException(nameof(umPayHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<UmPayDepositResponse?> DepositAsync(string merchantNo, string apiKey, DepositOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order, _jsonSerializerOptions);
            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson, _jsonSerializerOptions)!;

            var sign = _umPayHasher.ComputeHash(apiKey, orderDict);

            orderDict["sign"] = sign;

            var paramDict = orderDict
                .AsEnumerable()
                .Select(p => new KeyValuePair<string?, string?>(p.Key, p.Value?.ToString()));

            var content = new FormUrlEncodedContent(paramDict);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient
                .PostAsync("", content)
                .ConfigureAwait(false);

            _logger.LogInformation($"Deposit StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Deposit Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = JsonSerializer.Deserialize<UmPayDepositResponse>(bodyJson, _jsonSerializerOptions);

            return apiResponseData;
        }

        public async ValueTask<UmPayQueryExchangeRateResponse> QueryExchangeRateAsync(QueryExchangeRateOrder order)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(order, _jsonSerializerOptions));

            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
                .PostAsync("/get_rate", requestContent)
                .ConfigureAwait(false);

            _logger.LogInformation($"ExchangeRate StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"ExchangeRate Body: {bodyJson}");

            var result = JsonSerializer.Deserialize<UmPayQueryExchangeRateResponse>(bodyJson, _jsonSerializerOptions);

            return result!;
        }
    }
}