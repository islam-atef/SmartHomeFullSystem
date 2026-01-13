using Domain.Entities.SqlEntities.DiviceEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.RoomEntities
{
    public class Room : BaseEntity<Guid>
    {
        public Room() { }

        public string RoomName { get; private set; } = string.Empty;

        private readonly List<RoomProfile> _roomProfiles = new();
        public IReadOnlyCollection<RoomProfile> RoomProfiles => _roomProfiles.AsReadOnly();

        public bool IsShared => RoomProfiles.Count > 1;

        private readonly List<ControlUnit> _roomControlUnits = new();
        public IReadOnlyCollection<ControlUnit> RoomControlUnits => _roomControlUnits.AsReadOnly();

        public bool IsFunctional => RoomControlUnits.Count > 1;

        public Guid HomeId { get; set; }
        public Home Home { get; set; } = default!;



        public void Rename(string newName , string user)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new Exception("Room name cannot be empty");
            RoomName = newName.Trim(); 
            UpdateAudit(user);
        }

        public static Room Create(string name, Guid homeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Room name is required.", nameof(name));

            return new Room
            {
                Id = Guid.NewGuid(),
                RoomName = name.Trim(),
                HomeId = homeId
            };
        }

        public void AttachControlUnit(ControlUnit cu)
        {
            ArgumentNullException.ThrowIfNull(cu);
            if (cu.RoomId != Id) cu.MoveToRoom(Id);
            if (!_roomControlUnits.Contains(cu)) _roomControlUnits.Add(cu);
            ModifiedAt = DateTime.UtcNow;
        }

        public void DetachControlUnit(ControlUnit cu)
        {
            ArgumentNullException.ThrowIfNull(cu);
            _roomControlUnits.Remove(cu);
            ModifiedAt = DateTime.UtcNow;
        }

        internal void AttachProfileRoom(RoomProfile pr)
        {
            if (!_roomProfiles.Contains(pr)) _roomProfiles.Add(pr);
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
