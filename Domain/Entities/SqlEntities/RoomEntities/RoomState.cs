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
        private RoomState() { }

        public Guid RoomProfileId { get; private set; }
        public virtual RoomProfile RoomProfile { get; private set; } = default!;

        public DateTime? LastChange {get; private set; }

        public string StateData { get; private set; } = default!;

        public static RoomState Create(RoomProfile pr, string stateData)
        {
            if (pr is null)
                throw new ArgumentNullException(nameof(pr));

            if (string.IsNullOrWhiteSpace(stateData))
                throw new ArgumentException("State data is required", nameof(stateData));

            return new RoomState { 
                Id = Guid.NewGuid(),
                RoomProfile = pr,
                LastChange = null,
                StateData = stateData 
            };
        }

        public void UpdateState(string stateData)
        {
            if (string.IsNullOrWhiteSpace(stateData))
                throw new ArgumentException("State data is required", nameof(stateData));

            LastChange = DateTime.UtcNow;
            StateData = stateData;
        }
    }
}
