using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.RoomEntities
{
    public class ProfileRoom : BaseEntity<Guid>
    {
        public required Guid ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public virtual required Profile Profile { get; set; }

        public required Guid RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public virtual required Room Room { get; set; }

        public virtual List<RoomState>? FavoritStates { get; set; }

        public virtual RoomState? LastState { get; set; }
    }
}
