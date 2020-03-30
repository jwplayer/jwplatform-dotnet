using System;
using System.Security.Cryptography;
using System.Text;

namespace jwplatform
{
    internal static class ClientUtils
    {
        internal static string GenerateNonce()
        {
            return new Random().Next(999999999).ToString().PadLeft(9, '0');
        }

        internal static Int64 GetCurrentTimeInSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        internal static string EncodeString(string stringToEncode)
        {
            return Uri.EscapeDataString(stringToEncode);
        }

        internal static string GetSha1Hex(string stringToHex)
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToHex));
                var sha1Hex = new StringBuilder(hash.Length * 2);

                foreach (var x in hash)
                {
                    sha1Hex.Append(x.ToString("x2"));
                }

                return sha1Hex.ToString();
            }
        }
    }
}