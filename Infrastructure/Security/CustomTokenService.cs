using Application.Abstractions.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class CustomTokenService : ICustomTokenService
    {
        public string GenerateRefreshToken()
        {
            // 1- Generate a secure random 64-byte array 
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            // 2- Return as Base64 string
            return Convert.ToBase64String(randomBytes);
        }
    }
}
