namespace Tgs.Payment.UmPay
{
    public record UmPayDepositSettings(
        string ApiKey,
        string MerchantNo,
        string TradeType,
        string OrderType,
        string OrderNo,
        decimal Amount,
        decimal Rate,
        string TradeDesc);
}