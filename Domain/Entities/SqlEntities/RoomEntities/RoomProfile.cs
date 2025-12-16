using Application.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities.SqlEntities.RoomEntities
{
    public class RoomProfile : BaseEntity<Guid>
    {
        public Guid ProfileId { get; private set; }
        public virtual Profile Profile { get; private set; }

        public string Name { get; set; }

        public Guid RoomId { get; private set; }
        public virtual Room Room { get; private set; }

        private readonly List<RoomState> _favoriteStates = new();
        public IReadOnlyCollection<RoomState> FavoriteStates => _favoriteStates.AsReadOnly();

        public virtual RoomState? LastState { get; private set; }

        private RoomProfile() { }

        public static RoomProfile Link(Profile profile, Room room)
        {
            ArgumentNullException.ThrowIfNull(profile);
            ArgumentNullException.ThrowIfNull(room);

            var pr = new RoomProfile
            {
                Id = Guid.NewGuid(),
                Profile = profile,
                ProfileId = profile.Id,
                Room = room,
                RoomId = room.Id
            };
            room.AttachProfileRoom(pr);
            return pr; 
        }

        public RoomState AddFavoriteState(string stateData)
        {
            var st = RoomState.Create(this, stateData); 
            _favoriteStates.Add(st);
            ModifiedAt = DateTime.UtcNow;
            return st;
        }

        public void RemoveFavoriteState(RoomState state)
        {
            ArgumentNullException.ThrowIfNull(state);
            _favoriteStates.Remove(state); 
            ModifiedAt = DateTime.UtcNow;
        }

        public void SetLastState(RoomState state)
        {
            ArgumentNullException.ThrowIfNull(state);
            if (state.RoomProfileId != Id)
                throw new InvalidOperationException("State does not belong to this ProfileRoom.");
            LastState = state;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}

