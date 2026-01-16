using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.RuleServices
{
    public static class ReferenceHashMaker
    {
        public static string HomeReferenceMaker(Guid id, string iSO3166_2_lvl4)
        {
            if (id == Guid.Empty || String.IsNullOrEmpty(iSO3166_2_lvl4)) return string.Empty;

            var homeRef = $"{id:N}{iSO3166_2_lvl4}";

            // Hash to get deterministic output
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(homeRef));

            // Convert to digits only
            var digits = Regex.Replace(BitConverter.ToString(hash), "[^0-9]", "");

            // Ensure exactly 6 digits
            var code = digits.Length >= 6
                ? digits.Substring(0, 6)
                : digits.PadRight(6, '0');

            return $"#{code}";
        }
    }
}
