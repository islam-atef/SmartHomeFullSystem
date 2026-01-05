using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.SecurityEntities
{
    public class UserRefreshToken : BaseEntity<Guid>
    {
        public Guid AppUserId { get; set; }

        public string TokenHash { get; set; } = default!;
        public string TokenSalt { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public bool Revoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; } = null!;

        public Guid DeviceId { get; set; } = Guid.Empty;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !Revoked && !IsExpired;

        public static UserRefreshToken Create(Guid userId, string tokenHash, string tokenSalt, DateTime expiresAt, Guid deviceId )
        {
            if (userId == Guid.Empty || tokenHash == null || tokenSalt == null || expiresAt < DateTime.UtcNow)
                throw new ArgumentException("Wrong Data!");
            return new UserRefreshToken
            { 
                Id = Guid.NewGuid(),
                AppUserId = userId,
                TokenHash = tokenHash,
                TokenSalt = tokenSalt,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                DeviceId = deviceId,
            };
        }
    }
}
