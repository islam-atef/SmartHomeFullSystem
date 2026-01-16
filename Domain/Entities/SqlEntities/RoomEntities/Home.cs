using Domain.Entities.SqlEntities.UsersEntities;
using Domain.RuleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.RoomEntities
{
    public class Home : BaseEntity<Guid>
    {
        public Home() { }

        public string Name { get; private set; } = default!;
        public string HomeReference { get; private set; } = default!;

        private readonly List<Room> _homeRooms = new();
        public IReadOnlyCollection<Room> HomeRooms => _homeRooms.AsReadOnly();

        private readonly List<AppUser> _appUsers = new();
        public IReadOnlyCollection<AppUser> AppUsers => _appUsers.AsReadOnly();

        public Guid HomeOwnerId {  get; set; } = default;

        public double Latitude { get; private set; } = default!;   
        public double Longitude { get; private set; } = default!;  

        public string? HomeInfo { get; private set; } = default!;

        public string ISO3166_2_lvl4 { get; private set; } = default!;
        public string Country { get; private set; } = default!;
        public string State { get; private set; } = default!;
        public string Address { get; private set; } = default!;

        public void Rename(string newName , string userName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new Exception("Profile name cannot be empty");
            Name = newName.Trim();
            UpdateAudit(userName);
        }

        public static Home Create(
            string name , 
            string? info, 
            double latitude, 
            double longitude,
            string iSO3166_2_lvl4,
            string country,
            string state,
            string address,
            Guid homeOwner)
        {
            var id = Guid.NewGuid();

            var H = new Home
            {
                Id = id,
                Name = NormalizeName(name),
                ISO3166_2_lvl4 = iSO3166_2_lvl4,
                Country = country,
                State = state,
                Address = address,
                HomeReference = ReferenceHashMaker.HomeReferenceMaker(id, iSO3166_2_lvl4),
            };

            H.SetHomeInfo(info ?? null);
            H.SetHomeLocation(latitude, longitude);
            H.SetHomeOwner(homeOwner);
            return H;
        }

        public void SetHomeOwner(Guid owner)
        {
            HomeOwnerId = owner;
            ModifiedAt = DateTime.UtcNow;
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

        public void SetHomeInfo(string? info)
        {
            HomeInfo = info;
            ModifiedAt = DateTime.UtcNow;
        }

        public void SetHomeLocation(double latitude, double longitude)
        {
            if (double.IsNaN(latitude) || double.IsNaN(longitude))
                throw new ArgumentException(nameof(latitude), nameof(longitude));
            Latitude = latitude;    
            Longitude = longitude;
            ModifiedAt = DateTime.UtcNow;
        }

        private static string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("Name is required.");
            return s.Trim();
        }
    }
}
