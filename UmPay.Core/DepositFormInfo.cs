using System;
using Tgs.Payment.Clients;

namespace Tgs.Payment.UmPay;

public record DepositFormInfo(
   Guid Id,
   string PlayerCode,
   Guid BrandId,
   Guid BrandPaymentId,
   decimal Amount,
   PaymentFlowState Status,
   UmPayDepositApplyInfo? ApplyInfo,
   string? PaymentReturn = null);
