namespace Tgs.Payment.UmPay;

public class UmPayDepositReturn
{
    public string OutTradeNo { get; set; } = default!;

    public string TradeNo { get; set; } = default!;

    public string MerchantNo { get; set; } = default!;

    public string TradeStatus { get; set; } = default!;

    public decimal Amount { get; set; }

    public decimal RealAmount { get; set; }

    public string Sign { get; set; } = default!;

    public string Msg { get; set; } = default!;
}
