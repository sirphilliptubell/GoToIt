using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Converts the string to a base 64 encoded SHA256 hash.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        internal static string ToSha256Base64(this string s) {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var sha = SHA256.Create()) {
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }
    }
}
