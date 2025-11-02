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
        public required string DeiveMAC { get; set; }

        public string DeiveFamilyType { get; set; } = string.Empty;

        public string DeiveModel { get; set; } = string.Empty;

        public string DeiveName { get; set;} = string.Empty;

        public required Guid RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public virtual required Room Room { get; set; }
    }
}
