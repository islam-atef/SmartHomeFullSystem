using Domain.Entities.SqlEntities.RoomEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.DiviceEntities
{
    public class ControlUnit : BaseEntity<Guid>
    {
        public string UnitMAC { get;private set; } // could be modified

        public string UnitFamilyType { get; set; } = string.Empty;

        public string UnitModel { get; set; } = string.Empty;

        public string UnitPowerType {  get; set; } = string.Empty;

        public string UnitName { get; set;} = string.Empty;

        public Guid RoomId { get; private set; }
        public virtual Room Room { get; private set; }


        public void Rename(string newName, string user)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new Exception("Room name cannot be empty");
            UnitName = newName.Trim();
            UpdateAudit(user);
        }

        public static ControlUnit Create(string mac, string name, Guid roomId, string familyType = "", string model = "", string powerType = "")                           
        {
            var cu = new ControlUnit
            {
                Id = Guid.NewGuid(),
                UnitMAC = NormalizeName(name),
                UnitFamilyType = familyType?.Trim() ?? string.Empty,
                UnitPowerType = powerType?.Trim() ?? string.Empty,
                UnitName = model?.Trim() ?? string.Empty
            };
            cu.SetMac(mac);
            cu.MoveToRoom(roomId);
            return cu;
        }

        public void SetMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac)) throw new ArgumentException("MAC is required.", nameof(mac));
            var v = mac.Trim().ToUpperInvariant().Replace('-', ':');
            if (!System.Text.RegularExpressions.Regex.IsMatch(v, "^[0-9A-F]{2}(:[0-9A-F]{2}){5}$"))
                throw new ArgumentException("Invalid MAC format.", nameof(mac));
            UnitMAC = v;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateInfo(string? unitPowerType = null, string? name = null)
        {
            if (unitPowerType is not null) UnitPowerType = unitPowerType.Trim();
            if (name is not null) UnitName = name.Trim();
            ModifiedAt = DateTime.UtcNow;
        }

        public void MoveToRoom(Guid roomId)
        {
            if (roomId == Guid.Empty) throw new ArgumentException("RoomId required.", nameof(roomId));
            RoomId = roomId;
            ModifiedAt = DateTime.UtcNow;
        }

        private static string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("Name is required.");
            return s.Trim();
        }
    }
}
