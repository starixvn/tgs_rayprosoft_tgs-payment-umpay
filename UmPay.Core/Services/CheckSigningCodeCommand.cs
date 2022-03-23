using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tgs.Payment.UmPay.Services;

public record CheckSigningCodeCommand(
    IDictionary<string, object> Data,
    string SigningCode,
    string ApiKey)
{
    public TaskCompletionSource<bool> CommandResultSource { get; } = new TaskCompletionSource<bool>();
}
