using Microsoft.Extensions.Logging;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services;

internal class CheckSigningCodeCommandService : ICommandService<CheckSigningCodeCommand>
{
    private readonly UmPayHasher _hasher;
    private readonly ILogger<CheckSigningCodeCommandService> _logger;

    public CheckSigningCodeCommandService(UmPayHasher hasher, ILogger<CheckSigningCodeCommandService> logger)
    {
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask ExecuteAsync(
        CheckSigningCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("command.SigningCode: {command.SigningCode}", command.SigningCode);
        var signCode = _hasher.ComputeHash(command.ApiKey, command.Data);
        _logger.LogInformation("signCode: {signCode}", signCode);
        command.CommandResultSource.SetResult(signCode == command.SigningCode);

        return ValueTask.CompletedTask;
    }
}
