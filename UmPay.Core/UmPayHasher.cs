using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tgs.Payment.UmPay
{
    public class UmPayHasher
    {
        private readonly static MD5 _Md5;

        static UmPayHasher()
        {
            _Md5 = MD5.Create();
        }

        public string ComputeHash(string apiKey, IDictionary<string, object> dict)
        {            
            var content = string.Join(
                "",
                dict.OrderBy(x => x.Key)
                    .Select(p =>
                    {
                        if (p.Key == "sign" || p.Key == "rate")
                            return "";
                        else
                            return $"{p.Key}{p.Value}";
                    }));            

            var data = Encoding.UTF8.GetBytes(content);
            var base64 = Convert.ToBase64String(data);

            var fullText = $"{base64}{apiKey}";
            var hash = _Md5.ComputeHash(Encoding.UTF8.GetBytes(fullText));

            return Convert.ToHexString(hash).ToLower();
        }
    }
}