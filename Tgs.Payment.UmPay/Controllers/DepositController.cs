using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.UmPay.Services;
using Tgs.Payment.UmPay.ViewModels;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Controllers;

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
        [FromServices] ICommandService<CheckSigningCodeCommand> checkSigningCodeService,
        [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
        [FromServices] ICommandService<ApproveDepositFormCommand> approveService)
    {
        var form = Request.Form.Keys.ToDictionary(k => k, v => Request.Form[v].ToString());
        var json = JsonSerializer.Serialize(form);

        _logger.LogInformation("{json}", json);

        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        _ = dict.Remove("sign");
        _ = dict.Remove("msg");

        var depositResponse = JsonSerializer.Deserialize<CallbackDepositViewModel>(json)!;

        var depositForm = await getDepositFormService
            .GetAsync(new GetDepositFormQuery(depositResponse.OutTradeNo))
            .ConfigureAwait(false);

        var checkSigningCodeCommand = new CheckSigningCodeCommand(
            dict,
            depositResponse.Sign,
            depositForm.ApplyInfo!.Settings.ApiKey);

        await checkSigningCodeService
            .ExecuteAsync(checkSigningCodeCommand)
            .ConfigureAwait(false);

        if (await checkSigningCodeCommand.CommandResultSource.Task.ConfigureAwait(false))
        {
            if (depositForm.Status < PaymentFlowState.WaitApprove)
            {
                var returnData = new UmPayDepositReturn()
                {
                    OutTradeNo = depositResponse.OutTradeNo,
                    TradeNo = depositResponse.TradeNo,
                    MerchantNo = depositResponse.MerchantNo,
                    TradeStatus = depositResponse.TradeStatus,
                    Amount = decimal.Parse(depositResponse.Amount),
                    RealAmount = decimal.Parse(depositResponse.RealAmount),
                    Sign = depositResponse.Sign,
                    Msg = depositResponse.Msg
                };

                var saveReturnCommand = new SaveDepositReturnCommand(
                    depositForm.Id,
                    depositForm.Status,
                    returnData);

                await saveReturnService
                    .ExecuteAsync(saveReturnCommand)
                    .ConfigureAwait(false);

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var jsonText = JsonSerializer.Serialize(returnData, jsonSerializerOptions);
                    _logger.LogInformation("{jsonText}", jsonText);
                }

                await approveService
                    .ExecuteAsync(new ApproveDepositFormCommand(
                        depositForm.Id,
                        depositForm.BrandId,
                        returnData.RealAmount))
                    .ConfigureAwait(false);
            }

            _logger.LogInformation("callback executed");

            return "success";
        }
        else
            throw new Exception();
    }
}
