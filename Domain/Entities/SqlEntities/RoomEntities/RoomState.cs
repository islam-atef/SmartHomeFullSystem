using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.RoomEntities
{
    public class RoomState : BaseEntity<Guid>
    {
        public Guid ProfileRoomId { get; set; }
        [ForeignKey(nameof(ProfileRoomId))]
        public virtual required ProfileRoom ProfileRoom { get; set; }

        public DateTime? LastChange {get; set; }
    }
}
