namespace Tgs.Payment.UmPay;

public record UmPayDepositApplyInfo(
    UmPayDepositSettings Settings,
    UmPayDepositResponse Response);
