using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;

namespace Tgs.Payment.UmPay.MessageQueue;

internal class DepositScheduleSender
{
    private readonly IPaymentClient _client;
    private readonly ILogger<DepositScheduleSender> _logger;

    public DepositScheduleSender(
        IPaymentClient client,
        ILogger<DepositScheduleSender> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask SendScheduleResultAsync(string subject, Guid formId, DateTime scheduleTime)
    {
        _logger.LogInformation(
            "UmPay Send Schedule request Subject:{subject}, form Id:{formId}, Schedule Time:{scheduleTime:yyyy/MM/dd HH:mm:ss}",
            subject,
            formId,
            scheduleTime);

        await _client.ScheduleAsync(subject, formId, scheduleTime).ConfigureAwait(false);
    }
}
