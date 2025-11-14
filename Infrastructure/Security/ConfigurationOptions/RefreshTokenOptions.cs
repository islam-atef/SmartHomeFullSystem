using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security.ConfigurationOptions
{
    public sealed class RefreshTokenOptions
    {
        public int TokenSizeBytes { get; init; } = 64;  // entropy
        public int LifetimeDays { get; init; } = 7;
    }
}
