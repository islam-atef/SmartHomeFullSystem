using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security.ConfigurationOptions
{
    public sealed class JwtOptions
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string SigningKey { get; init; } = string.Empty;
        public int AccessTokenMinutes { get; init; } = 10;
    }
}
