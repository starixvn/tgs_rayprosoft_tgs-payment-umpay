using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.UmPay
{
    public class UmPayClient
    {
        private readonly HttpClient _httpClient;        
        private readonly UmPayOptions _options;
        private readonly UmPayHasher _umPayHasher;
        private readonly ILogger<UmPayClient> _logger;

        public UmPayClient(
            HttpClient httpClient,
            IOptions<UmPayOptions> optionsAccessor,
            UmPayHasher umPayHasher,
            ILogger<UmPayClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));            
            _options = optionsAccessor.Value;
            _umPayHasher = umPayHasher ?? throw new ArgumentNullException(nameof(umPayHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<UmPayDepositResponse?> DepositAsync(string merchantNo, string apiKey, DepositOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order, _options.SystemSerializerOptions);
            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson, _options.SystemSerializerOptions)!;

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

            var apiResponseData = JsonSerializer.Deserialize<UmPayDepositResponse>(bodyJson, _options.SystemSerializerOptions);

            return apiResponseData;
        }

        public async ValueTask<UmPayQueryExchangeRateResponse> QueryExchangeRateAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.PaymentApiUrl}/get_rate"),
                Content = new StringContent("some json", Encoding.UTF8, MediaTypeNames.Application.Json),
            };

            var response = await _httpClient
                .SendAsync(request)
                .ConfigureAwait(false);

            _logger.LogInformation($"ExchangeRate StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"ExchangeRate Body: {bodyJson}");

            var result = JsonSerializer.Deserialize<UmPayQueryExchangeRateResponse>(bodyJson, _options.SystemSerializerOptions);

            return result!;
        }

        public async ValueTask<UmPayCancelDepositResponse?> CancelDepositAsync(string apiKey, CancelDepositOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order, _options.SystemSerializerOptions);
            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson, _options.SystemSerializerOptions)!;
            
            var sign = _umPayHasher.ComputeHash(apiKey, orderDict);

            orderDict["sign"] = sign;

            var paramDict = orderDict
                .AsEnumerable()
                .Select(p => new KeyValuePair<string?, string?>(p.Key, p.Value?.ToString()));

            var content = new FormUrlEncodedContent(paramDict);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient
                .PostAsync("/cancel_order", content)
                .ConfigureAwait(false);

            _logger.LogInformation($"Cancel Deposit StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Cancel Deposit Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = JsonSerializer.Deserialize<UmPayCancelDepositResponse>(bodyJson, _options.SystemSerializerOptions);

            return apiResponseData;
        }
    }
}