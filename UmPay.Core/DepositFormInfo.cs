using System;

namespace Tgs.Payment.UmPay
{
    public record DepositFormInfo(
       Guid Id,
       string PlayerCode,
       Guid BrandId,
       Guid BrandPaymentId,
       decimal Amount,
       int Status,
       UmPayDepositApplyInfo? ApplyInfo,
       string? PaymentReturn = null);
}