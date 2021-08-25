using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.Payment.UmPay.Services;
using Tgs.Payment.UmPay.ViewModels;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly ILogger<DepositController> _logger;

        public DepositController(
            ILogger<DepositController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("callback")]
        [Produces("text/plain")]
        public async Task<string> DepositCallback(
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
            [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
            [FromForm] CallbackDepositViewModel request,
            [FromForm] IFormCollection formCollection)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var formData = string.Join(", ", formCollection.Select(item => $"{item.Key}:{item.Value}"));

                _logger.LogInformation(
                    $"Callback execute, request: " +
                    $"{formData}");
            }

            var depositForm = await getDepositFormService
                .GetAsync(new GetDepositFormQuery(request.OutTradeNo))
                .ConfigureAwait(false);

            if (depositForm.Status < 1000)
            {
                var callbackMessage = new UmPayDepositReturn()
                {
                    OutTradeNo = request.OutTradeNo,
                    TradeNo = request.TradeNo,
                    MerchantNo = request.MerchantNo,
                    TradeStatus = request.TradeStatus,
                    Amount = request.Amount,
                    RealAmount = request.RealAmount,
                    Sign = request.Sign,
                    Msg = request.Msg
                };

                await saveReturnService
                    .ExecuteAsync(new SaveDepositReturnCommand(depositForm.Id, callbackMessage))
                    .ConfigureAwait(false);

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                _logger.LogInformation(JsonSerializer.Serialize(callbackMessage, jsonSerializerOptions));

                await approveService
                    .ExecuteAsync(new ApproveDepositFormCommand(depositForm.Id, depositForm.BrandId))
                    .ConfigureAwait(false);
            }

            return "success";
        }
    }
}