using System;

namespace Tgs.Payment.UmPay.Services;

public record ApproveDepositFormCommand(Guid FormId, Guid BrandId, decimal? ActualAmount);
