using Application.Entities.SqlEntities;
using Application.Entities.SqlEntities.DiviceEntities;
using Application.Entities.SqlEntities.SecurityEntities;
using Application.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.UsersEntities
{
    public class AppDevice : BaseEntity<Guid>
    {
        public AppDevice() { }

        public string DeviceName { get; set; } = default!;
        public string DeviceType { get; set; } = default!;
        public string DeviceMACAddress { get; set; } = default!;
        public string DeviceIP { get; set; } = default!;

        private readonly List<AppUser> _deviceUsers = new();
        public IReadOnlyCollection<AppUser> DeviceUsers => _deviceUsers.AsReadOnly();

        private readonly List<UserRefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<UserRefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        public static AppDevice Create( string deviceMACAddress, string deviceIP, string name, string type)
        {
            if (String.IsNullOrEmpty(deviceMACAddress)) throw new ArgumentNullException("Invalid Date!");
            return new AppDevice
            { Id = Guid.NewGuid(), DeviceName = name, DeviceType = type, DeviceMACAddress = deviceMACAddress, DeviceIP = deviceIP };
        }

        public void UpdateIP(string deviceIP)
        {
            DeviceIP = deviceIP;
            UpdateAudit();
        }

        public void UpdateMAC(string deviceMAC)
        {
            DeviceMACAddress = deviceMAC;
            UpdateAudit();
        }

        public void AddToken(UserRefreshToken token)
        {
            if (token == null) throw new ArgumentNullException("No Token has been Provided!");
            _refreshTokens.Add(token);
            UpdateAudit();
        }

        public void AddUser(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("No User has been Provided!");
            _deviceUsers.Add(user);
            UpdateAudit();
        }
    }
}
