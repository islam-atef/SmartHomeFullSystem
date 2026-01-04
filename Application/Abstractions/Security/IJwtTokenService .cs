using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Security.Interfaces
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresUtc) GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
