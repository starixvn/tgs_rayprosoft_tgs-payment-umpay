using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.UmPay;

public class UmPayHasher
{
    private static readonly MD5 _Md5 = MD5.Create();
    private readonly ILogger<UmPayHasher> _logger;

    public UmPayHasher(ILogger<UmPayHasher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string ComputeHash(string apiKey, IDictionary<string, object> dict)
    {
        var content = string.Join(
            "",
            dict.OrderBy(x => x.Key)
                .Select(p =>
                {
                    return p.Key is "sign" or "rate" ? "" : $"{p.Key}{p.Value}";
                }));

        _logger.LogTrace("Content: {content}", content);

        var data = Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(data);

        var fullText = $"{base64}{apiKey}";

        _logger.LogTrace("FullText: {fullText}", fullText);

        var hash = _Md5.ComputeHash(Encoding.UTF8.GetBytes(fullText));

        return Convert.ToHexString(hash).ToLower();
    }
}
