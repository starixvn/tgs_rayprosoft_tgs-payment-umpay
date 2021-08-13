using System;
using System.Threading.Tasks;
using Tgs.Payment.UmPay.Repositories.Entities;

namespace Tgs.Payment.UmPay.Repositories
{
    internal class TgsRepository : ITgsRepository
    {
        private readonly TgsContext _dbContext;
        private readonly UmPaySerializer _serializer;

        public TgsRepository(TgsContext dbContext, UmPaySerializer serializer)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id)
        {
            var data = await _dbContext.DepositForms.FindAsync(id).ConfigureAwait(false);

            return new DepositFormInfo(
               data.Id,
               data.PlayerCode,
               data.BrandId,
               data.BrandPaymentId,
               data.Amount,
               data.Status,
               _serializer.Deserialize<UmPayDepositApplyInfo>(data.ApplyInfo!),
               data.PaymentReturn);
        }

        public async ValueTask SaveDepositReturnAsync(Guid id, UmPayDepositReturn depositReturn)
        {
            var form = await _dbContext.DepositForms
                .FindAsync(id)
                .ConfigureAwait(false);

            form.Status = 1000;
            form.ActualAmount = depositReturn.Amount;
            form.PaymentReturn = _serializer.Serialize(depositReturn);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}