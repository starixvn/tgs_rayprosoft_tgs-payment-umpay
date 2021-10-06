using Microsoft.AspNetCore.Mvc;

namespace Tgs.Payment.UmPay.ViewModels
{
    public class CallbackDepositViewModel
    {
        [FromForm(Name = "outTradeNo")]
        public string OutTradeNo { get; set; } = default!;

        [FromForm(Name = "tradeNo")]
        public string TradeNo { get; set; } = default!;

        [FromForm(Name = "merchantNo")]
        public string MerchantNo { get; set; } = default!;

        [FromForm(Name = "tradeStatus")]
        public int TradeStatus { get; set; }

        [FromForm(Name = "amount")]
        public decimal Amount { get; set; }

        [FromForm(Name = "real_amount")]
        public decimal RealAmount { get; set; }

        [FromForm(Name = "sign")]
        public string Sign { get; set; } = default!;

        [FromForm(Name = "msg")]
        public string Msg { get; set; } = default!;
    }
}