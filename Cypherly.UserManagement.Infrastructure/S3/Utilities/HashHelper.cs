using System.Security.Cryptography;
using System.Text;

namespace Cypherly.UserManagement.Infrastructure.S3.Utilities;

public static class HashHelper
{
    public static string GenerateHash(string input, bool urlSafe = true)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        var base64 = Convert.ToBase64String(hash);
        return urlSafe ? ToUrlSafeBase64(base64) : base64;
    }

    private static string ToUrlSafeBase64(string base64)
    {
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}