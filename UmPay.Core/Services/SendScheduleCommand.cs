using System;

namespace Tgs.Payment.UmPay.Services;

public record SendScheduleCommand(string Subject, Guid FormId, DateTime ScheduleTime);
