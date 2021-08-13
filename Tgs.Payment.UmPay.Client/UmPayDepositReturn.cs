namespace Tgs.Payment.UmPay
{
    public class UmPayDepositReturn
    {
        public string MerchantNo { get; set; } = default!;
        public string TradeType { get; set; } = default!;
        public string OrderType { get; set; } = default!;
        public string TimeStamp { get; set; } = default!;
        public string OrderNo { get; set; } = default!;
        public decimal Amount { get; set; } = default!;
        public decimal Rate { get; set; } = default!;
        public string TradeDesc { get; set; } = default!;
        public string NotifyUrl { get; set; } = default!;
        public string Sign { get; set; } = default!;
    }
}