using Domain.Entities.RedisEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Cashing.interfaces
{
    public interface IOtpDeviceCacheStore
    {
        Task SaveAsync(OtpDeviceChallengeCache entry, CancellationToken ct = default);
        Task<OtpDeviceChallengeCache?> GetAsync(Guid challengeId, CancellationToken ct = default);
        Task RemoveAsync(Guid challengeId, CancellationToken ct = default);
    }
}
