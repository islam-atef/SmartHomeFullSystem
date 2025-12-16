using Application.Entities.SqlEntities.RoomEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities.SqlEntities.UsersEntities
{
    public class Profile : BaseEntity<Guid>
    {
        public required string  ProfileName { get; set; }

        public required Guid UserId { get; init; }
        public required AppUser User { get; init; }

        public required Guid HomeId { get; init; }
        public required Home Home { get; init; }

        public virtual ICollection<RoomProfile> RoomProfiles { get; set; } = new HashSet<RoomProfile>();

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new Exception("Profile name cannot be empty");
            ProfileName = newName.Trim();
            UpdateAudit(User.IdentityUser.UserName);
        }
    }
}
