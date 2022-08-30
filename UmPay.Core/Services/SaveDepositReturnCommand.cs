using System;
using Tgs.Payment.Clients;

namespace Tgs.Payment.UmPay.Services;

public record SaveDepositReturnCommand(
    Guid FormId,
    PaymentFlowState State,
    UmPayDepositReturn DepositReturn);
