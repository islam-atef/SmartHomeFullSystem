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

        public Guid ProfileRoomId { get; private set; }
        [ForeignKey(nameof(ProfileRoomId))]
        public virtual RoomProfile ProfileRoom { get; private set; }

        public DateTime? LastChange {get; private set; }

        public string StateData { get; private set; }

        public static RoomState Create(RoomProfile pr, string stateData)
        {
            if (pr is null)
                throw new ArgumentNullException(nameof(pr));

            if (string.IsNullOrWhiteSpace(stateData))
                throw new ArgumentException("State data is required", nameof(stateData));

            return new RoomState { 
                Id = Guid.NewGuid(), 
                ProfileRoom = pr,
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
