using Domain.Entities.SqlEntities.RoomEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.UsersEntities
{
    public class Profile : BaseEntity<Guid>
    {
        public required string  ProfileName { get; set; }

        public required Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public required AppUser User { get; set; }

        public virtual ICollection<ProfileRoom> ProfileRooms { get; set; } = new HashSet<ProfileRoom>();

    }
}
