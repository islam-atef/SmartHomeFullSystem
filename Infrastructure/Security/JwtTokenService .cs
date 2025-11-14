using Application.Abstractions.Security.Interfaces;
using Infrastructure.Security.ConfigurationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwt;
        private readonly IConfiguration _config;
        private readonly TimeProvider _clock;
        private readonly JwtSecurityTokenHandler _handler = new();  
        public JwtTokenService(IConfiguration config, IOptions<JwtOptions> jwtOptions, TimeProvider clock)    
        { 
            _config = config;
            _jwt = jwtOptions.Value;
            _clock = clock;
        }

        public (string token, DateTime expiresUtc) GenerateAccessToken(IEnumerable<Claim> claims)
        {
            if (string.IsNullOrWhiteSpace(_jwt.SigningKey))
                throw new InvalidOperationException("JWT SigningKey is not configured.");

            var keyBytes = Encoding.UTF8.GetBytes(_jwt.SigningKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = _clock.GetUtcNow().UtcDateTime;
            var expires = now.AddMinutes(_jwt.AccessTokenMinutes);


            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials);

            var tokenString = _handler.WriteToken(jwt);
            return (tokenString, expires);
        }
    }
}
