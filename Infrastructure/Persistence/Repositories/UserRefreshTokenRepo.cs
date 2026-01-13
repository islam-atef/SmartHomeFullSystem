using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.GenericResult;
using Domain.RepositotyInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRefreshTokenRepo : IUserRefreshTokenRepo
    {
        private readonly AppDbContext _context;

        public UserRefreshTokenRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        {
            return await _context.UserRefreshTokens
           .AsNoTracking()
           .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);
        }

        public async Task<IEnumerable<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.UserRefreshTokens
           .AsNoTracking()
           .Where(t => t.AppUserId == userId)
           .OrderByDescending(t => t.ExpiresAt)
           .ToListAsync(ct);
        }

        public async Task<int> RemoveExpiredTokensAsync(CancellationToken ct = default)
        {
            var utcNow = DateTime.UtcNow;

            var affected = await _context.UserRefreshTokens
                .Where(t => t.ExpiresAt <= utcNow)
                .ExecuteDeleteAsync(ct);
            return affected;
        }

        public async Task<GenericResult<int>> RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = await _context.UserRefreshTokens
                .Where(t => t.AppUserId == userId && !t.Revoked)
                .ToListAsync(ct);

            if (tokens.Count == 0)
                return GenericResult<int>.Success(0, "No active tokens to revoke.");

            var now = DateTime.UtcNow;
            foreach (var t in tokens)
            {
                t.Revoked = true;
                t.RevokedAt = now;
            }

            var affected = await _context.SaveChangesAsync(ct) >0;
            return affected
                ? GenericResult<int>.Success(tokens.Count, "All user tokens revoked.")
                : GenericResult<int>.Failure(ErrorType.DatabaseError, "Can not save changes!");
        }

        public async Task<GenericResult<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken ct = default)
        {
            var token = await _context.UserRefreshTokens.FirstOrDefaultAsync(t => t.Id == tokenId, ct);
            if (token is null)
                return GenericResult<bool>.Failure(ErrorType.NotFound, "Refresh token not found.");
            if (!token.Revoked)
            {
                token.Revoked = true;
                token.RevokedAt = DateTime.UtcNow;
                var saved = await _context.SaveChangesAsync(ct) > 0;
                return saved
                    ? GenericResult<bool>.Success(true, "Refresh token revoked.")
                    : GenericResult<bool>.Failure(ErrorType.DatabaseError, "Failed to save refresh token.");
            }
            return GenericResult<bool>.Success(true, "Refresh token is aready revoked.");
        }

        public async Task<GenericResult<UserRefreshToken>> SaveTokenAsync(
            Guid userId,
            string TKSalt, string TKHash, int expiredAfter,
            Guid deviceId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(TKSalt) || string.IsNullOrWhiteSpace(TKHash) || expiredAfter <= 0 || userId == Guid.Empty)
            {
                return GenericResult<UserRefreshToken>.Failure(ErrorType.InvalidData, "Invalid token data provided.");
            }
            try
            {
                var token = UserRefreshToken.Create(userId,TKHash,TKSalt,DateTime.UtcNow.AddDays(expiredAfter), deviceId);
                await _context.UserRefreshTokens.AddAsync(token, ct);
                var savedResult = await _context.SaveChangesAsync(ct);
                return savedResult > 0
                    ? GenericResult<UserRefreshToken>.Success(token, "Refresh token saved.")
                    : GenericResult<UserRefreshToken>.Failure(ErrorType.DatabaseError, "Failed to save refresh token.");
            }
            catch (Exception ex)
            {
                return GenericResult<UserRefreshToken>.Failure(ErrorType.Conflict, ex.Message);
            }
        }

        public async Task<GenericResult<string>> RotateRT_DBProcessAsync(Guid oldTokenId, Guid userId, Guid newTokenID, CancellationToken ct = default)
        {
            // get the old token
            var oldToken = await _context.UserRefreshTokens.FirstOrDefaultAsync(t => t.Id == oldTokenId && t.AppUserId == userId, ct);
            if (oldToken is null)
                return GenericResult<string>.Failure(ErrorType.NotFound, "Old refresh token not found.");

            if (oldToken.Revoked || oldToken.ExpiresAt <= DateTime.UtcNow)
                return GenericResult<string>.Failure(ErrorType.InvalidData, "Old token already invalid.");

            // attach the two token together
            oldToken.Revoked = true;
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByToken = newTokenID.ToString();

            // save
            var saved = await _context.SaveChangesAsync(ct) > 0;
            if (saved)
                return GenericResult<string>.Success("Done", "Refresh token rotated DB process Successeded.");
            else
                return GenericResult<string>.Failure(ErrorType.Conflict, "Can not save changes!.");
        } 
    }
}
