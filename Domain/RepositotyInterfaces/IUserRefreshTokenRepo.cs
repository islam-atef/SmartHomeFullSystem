using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.GenericResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IUserRefreshTokenRepo
    {
        Task<GenericResult<UserRefreshToken>> SaveTokenAsync(UserRefreshToken token, CancellationToken ct = default);

        Task<GenericResult<string>> RotateRefreshTokenAsync(Guid oldTokenId, Guid userId, string newSalt, string newHash, int expiredAfter, CancellationToken ct = default);

        Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

        Task<IEnumerable<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken ct = default);

        Task<GenericResult<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken ct = default);

        Task<GenericResult<int>> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default);

        Task<int> RemoveExpiredTokensAsync(CancellationToken ct = default);
    }
}
