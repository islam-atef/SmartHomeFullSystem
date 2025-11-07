using Application.Entities.SqlEntities.DiviceEntities;
using Application.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities.SqlEntities.RoomEntities
{
    public class Home : BaseEntity<Guid>
    {
        public Home() { }

        public string Name { get;private set; }

        private readonly List<Room> _homeRooms = new();
        public IReadOnlyCollection<Room> HomeRooms => _homeRooms.AsReadOnly();

        private readonly List<AppUser> _appUsers = new();
        public IReadOnlyCollection<AppUser> AppUsers => _appUsers.AsReadOnly();

        public string HomeIP { get; private set; }

        public void Rename(string newName , string userName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new Exception("Profile name cannot be empty");
            Name = newName.Trim();
            UpdateAudit(userName);
        }

        public static Home Create(string name , string ip)
        {
            var H = new Home
            {
                Id = Guid.NewGuid(),
                Name = NormalizeName(name),
            };
            H.SetHomeIP(ip);
            return H;
        }

        public void AddUser(AppUser user)
        {
            _appUsers.Add(user);
            ModifiedAt = DateTime.UtcNow;
        }

        public void RemoveUser(AppUser user)
        {
            _appUsers.Remove(user);
            ModifiedAt = DateTime.UtcNow;
        }

        public void AddRoom(Room room)
        {
            _homeRooms.Add(room);
            ModifiedAt = DateTime.UtcNow;
        }

        public void RemoveRoom(Room room)
        {
            _homeRooms.Remove(room);
            ModifiedAt = DateTime.UtcNow;
        }

        public void SetHomeIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) throw new ArgumentException("IP is required.", nameof(ip));
            var v = ip.Trim();

            if (!System.Net.IPAddress.TryParse(v, out var parsedIp))
                throw new ArgumentException("Invalid IP address format.", nameof(ip));

            HomeIP = parsedIp.ToString();
            ModifiedAt = DateTime.UtcNow;
        }

        private static string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("Name is required.");
            return s.Trim();
        }
    }
}
