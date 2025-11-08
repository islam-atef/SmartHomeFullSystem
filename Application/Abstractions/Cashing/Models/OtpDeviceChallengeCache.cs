using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.RedisEntities
{
    public class OtpDeviceChallengeCache
    {
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        public string OtpCodeHash { get; set; } = default!;
        public string OtpSalt { get; set; } = default!;
        public string? Email { get; init; } = default;
        public string DeviceIP { get; init; } = default!;
        public string DeviceMACAddress { get; set; } = default!;

        public static OtpDeviceChallengeCache Create(Guid appUserId, string otpCodeHash, string otpSalt, string deviceIP, string deviceMACAddress, string? email = default)
            => new OtpDeviceChallengeCache
            {
                Id = Guid.NewGuid(),
                AppUserId = appUserId,
                OtpCodeHash = otpCodeHash,
                OtpSalt = otpSalt,
                DeviceIP = deviceIP,
                DeviceMACAddress = deviceMACAddress,
                Email = email
            };
    }
}
