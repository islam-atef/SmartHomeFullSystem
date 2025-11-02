using Domain.Entities.SqlEntities.DiviceEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.RoomEntities
{
    public class Room : BaseEntity<Guid>
    {
        public required string RoomName { get; set; }

        public virtual ICollection<ProfileRoom> RoomProfiles { get; set; } = new HashSet<ProfileRoom>();

        public virtual ICollection<ControlUnit> RoomControlUnits { get; set; } = new HashSet<ControlUnit>();
    }
}
