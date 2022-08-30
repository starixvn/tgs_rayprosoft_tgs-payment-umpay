using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.UmPay.MessageQueue;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services;

internal class SendScheduleCommandService : ICommandService<SendScheduleCommand>
{
    private readonly DepositScheduleSender _sender;

    public SendScheduleCommandService(DepositScheduleSender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public async ValueTask ExecuteAsync(SendScheduleCommand command, CancellationToken cancellationToken = default)
        => await _sender
            .SendScheduleResultAsync(command.Subject, command.FormId, command.ScheduleTime)
            .ConfigureAwait(false);
}
