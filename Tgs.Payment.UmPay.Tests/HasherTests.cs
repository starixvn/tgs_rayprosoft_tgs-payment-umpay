using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Tgs.Payment.UmPay.Tests;

[TestFixture]
public class HasherTests
{
    [Test]
    public void Test_Check_Hash()
    {
        var merchantNo = "MAJA1628579663";
        var tradeType = "PAY01";
        var orderType = 1;
        var timeStamp = "1628585233";
        var orderNo = "0123987654";//商戶訂單號 請帶純數字
        var amount = 100;
        var tradeDesc = "order123";
        var notifyUrl = "https://hk.um-pay.com/welcome/callback";

        var dict = new Dictionary<string, object>
        {
            { "merchantNo", merchantNo},
            { "tradeType", tradeType},
            { "orderType",orderType},
            { "timeStamp", timeStamp},
            { "orderNo", orderNo},
            { "amount" , amount},
            { "tradeDesc", tradeDesc},
            { "notifyUrl", notifyUrl}
        };

        var hasher = new UmPayHasher(NullLogger<UmPayHasher>.Instance);
        var apiKey = "7bdc9a41b4d54ca1f593cddf00f49023";
        var actual = hasher.ComputeHash(apiKey, dict);

        var expected = "dea5e91713048526f7fcf1dd1c1ec2e0";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Verify_Callback_Hash()
    {
        var amount = "1000.00";
        var tradeNo = "AH16345227581984";
        var merchantNo = "MAJA1628579663";
        var outTradeNo = "25292327378258416915355444242454340061";
        var realAmount = "1000.00";
        var tradeStatus = 2;

        var dict = new Dictionary<string, object>
        {
            { "amount", amount },
            { "tradeNo",tradeNo },
            { "merchantNo", merchantNo },
            { "outTradeNo", outTradeNo },
            { "real_amount" , realAmount },
            { "tradeStatus", tradeStatus }
        };

        var hasher = new UmPayHasher(NullLogger<UmPayHasher>.Instance);
        var apiKey = "7bdc9a41b4d54ca1f593cddf00f49023";
        var actual = hasher.ComputeHash(apiKey, dict);

        var expected = "becb54ac0ca26a1cbbad5e072011841d";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Verify_Cancel_Order_Hash()
    {
        var orderNo = "01239876543";
        var merchantNo = "MAJA1628579663";

        var dict = new Dictionary<string, object>
        {
            { "orderNo",orderNo },
            { "merchantNo", merchantNo }
        };

        var hasher = new UmPayHasher(NullLogger<UmPayHasher>.Instance);
        var apiKey = "7bdc9a41b4d54ca1f593cddf00f49023";
        var actual = hasher.ComputeHash(apiKey, dict);

        var expected = "aba1e6f8235c7613a96cb34a1a15827d";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Max_Guid_To_UnSign_Integer()
    {
        var guid = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var bigInteger = new BigInteger(
            guid.ToByteArray(),
            isUnsigned: true,
            isBigEndian: false);

        Assert.AreEqual(
            BigInteger.Parse("340282366920938463463374607431768211455"),
            bigInteger);
    }

    [Test]
    public void Max_Integer_To_Guid()
    {
        var numberText = "340282366920938463463374607431768211455";

        var bigInt = BigInteger.Parse(numberText, System.Globalization.NumberStyles.None);

        var expected = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        //important
        var buffer = new Span<byte>(new byte[16]);
        bigInt.ToByteArray().Take(16).ToArray().CopyTo(buffer);
        var actual = new Guid(buffer);

        Assert.AreEqual(expected, actual);
    }
}
