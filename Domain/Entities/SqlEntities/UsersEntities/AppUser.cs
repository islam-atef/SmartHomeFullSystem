using Domain.Entities.SqlEntities.RoomEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.UsersEntities
{
    public class AppUser : BaseEntity<Guid>
    {
        public string? UserImageUrl { get; set; }

        public required Guid IdentityUserId { get; set; }
        public virtual AppIdentityUser IdentityUser { get; set; } = default!;

        public virtual ICollection<Profile>? Profiles { get; set; } = new HashSet<Profile>();

        public virtual ICollection<Home>? Homes { get; set; } = new HashSet<Home>();
    }
}
