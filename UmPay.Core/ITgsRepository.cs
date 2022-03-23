using System;
using System.Threading.Tasks;

namespace Tgs.Payment.UmPay;

public interface ITgsRepository
{
    ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id);

    ValueTask SaveDepositReturnAsync(Guid id, UmPayDepositReturn depositReturn);
}
