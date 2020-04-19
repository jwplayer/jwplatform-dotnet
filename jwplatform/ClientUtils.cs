using System;
using System.Security.Cryptography;
using System.Text;

namespace jwplatform
{
    /// <summary>
    /// Contains helper functions used by the <c>Client</c> class.
    /// </summary>
    internal static class ClientUtils
    {
        /// <summary>
        /// Generates a random 8 digit number as a string.
        /// </summary>
        /// <returns> The 8 digit number as a string. </returns>
        internal static string GenerateNonce()
        {
            return new Random().Next(99999999).ToString().PadLeft(8, '0');
        }

        /// <summary>
        /// Gets the current UNIX timestamp in seconds.
        /// </summary>
        /// <returns> The seconds as an Int64. </returns>
        internal static Int64 GetCurrentTimeInSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Encodes <paramref name="stringToEncode" /> by converting characters to their hexidecimal representation.
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.uri.escapedatastring?view=netframework-4.8">
        /// MSDN Docs - URI.EscapeDataString
        /// </see>
        /// </summary>
        /// <param name="stringToEncode"> A string to encode. </param>
        /// <returns> <paramref name="stringToEncode" /> as an encoded string. </returns>
        internal static string Encode(string stringToEncode)
        {
            return Uri.EscapeDataString(stringToEncode);
        }

        /// <summary>
        /// Generates a secure hash from <paramref name="stringToHex" />.
        /// </summary>
        /// <param name="stringToHex"> A string to hash. </param>
        /// <returns> The generated hash as a string. </returns>
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