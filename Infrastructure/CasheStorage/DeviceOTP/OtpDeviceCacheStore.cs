using Application.Abstractions.CasheStorage.DeviceOTP;
using Application.Contracts.DeviceChecking.OtpChallenge;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Storage.DeviceOTP
{
    public class OtpDeviceCacheStore : IOtpDeviceCacheStore
    {
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(2);
        private readonly IDatabase _redis;

        public OtpDeviceCacheStore(IConnectionMultiplexer connection)
            => _redis = connection.GetDatabase();

        private static string Key(Guid id) => $"otp:device:{id}";

        public async Task SaveAsync(OtpDeviceChallengeCache entry, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(entry);
            await _redis.StringSetAsync(Key(entry.Id), json, _defaultExpiration);
        }

        public async Task<OtpDeviceChallengeCache?> GetAsync(Guid challengeId, CancellationToken ct = default)
        {
            var value = await _redis.StringGetAsync(Key(challengeId));
            return value.HasValue ? JsonSerializer.Deserialize<OtpDeviceChallengeCache>(value!) : null;
        }

        public Task RemoveAsync(Guid challengeId, CancellationToken ct = default)
            => _redis.KeyDeleteAsync(Key(challengeId));
    }
}
