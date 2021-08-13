using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<string> DepositCallback(
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
            [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
            [FromForm] CallbackDepositViewModel request)
        {
            var depositForm = await getDepositFormService
                .GetAsync(new GetDepositFormQuery(request.OrderNo))
                .ConfigureAwait(false);

            if (depositForm.Status < 1000)
            {
                var callbackMessage = new UmPayDepositReturn()
                {
                    MerchantNo = request.MerchantNo,
                    TradeType = request.TradeType,
                    OrderType = request.OrderType,
                    TimeStamp = request.TimeStamp,
                    OrderNo = request.OrderNo,
                    Amount = request.Amount,
                    Rate = request.Rate,
                    TradeDesc = request.TradeDesc,
                    NotifyUrl = request.NotifyUrl,
                    Sign = request.Sign
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