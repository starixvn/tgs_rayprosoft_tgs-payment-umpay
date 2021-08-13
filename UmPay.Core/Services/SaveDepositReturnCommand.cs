using System;

namespace Tgs.Payment.UmPay.Services
{
    public record SaveDepositReturnCommand(Guid FormId, UmPayDepositReturn DepositReturn);
}