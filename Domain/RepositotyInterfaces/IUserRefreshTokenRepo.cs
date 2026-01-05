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
        Task<GenericResult<UserRefreshToken>> SaveTokenAsync(
            Guid userId,
            string TKSalt, string TKHash, int expiredAfter,
            Guid deviceId ,
            CancellationToken ct = default);

        Task<GenericResult<string>> RotateRT_DBProcessAsync(Guid oldTokenId, Guid userId, Guid newTokenID , CancellationToken ct = default);

        Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

        Task<IEnumerable<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken ct = default);

        Task<GenericResult<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken ct = default);

        Task<GenericResult<int>> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default);

        Task<int> RemoveExpiredTokensAsync(CancellationToken ct = default);
    }
}
