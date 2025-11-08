using Application.RuleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class HashingService : IHashingService
    {
        private const int SaltSize = 32;         // 256 bits
        private const int HashSize = 32;         // 256 bits
        private const int Iterations = 150_000;  // PBKDF2 rounds

        public (byte[] Hash, byte[] Salt) Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            // 1️-  Generate a random salt
            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // 2️-  Derive the hash using PBKDF2 (HMAC-SHA256)
            using var pbkdf2 = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(input),
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(HashSize);
            return (hash, salt);
        }

        public bool Verify(string input, byte[] hash, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes(input),
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

            var computed = pbkdf2.GetBytes(HashSize);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
