using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace Tgs.Payment.UmPay.Services
{
    internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetDepositFormQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public ValueTask<DepositFormInfo> GetAsync(GetDepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var bigInt = BigInteger.Parse(query.OrderNo, System.Globalization.NumberStyles.None);
            var buffer = new Span<byte>(new byte[16]);
            bigInt.ToByteArray().Take(16).ToArray().CopyTo(buffer);
            var tradeNo = new Guid(buffer);

            return _tgsRepository
                .GetDepositFormByIdAsync(tradeNo);
        }
    }
}