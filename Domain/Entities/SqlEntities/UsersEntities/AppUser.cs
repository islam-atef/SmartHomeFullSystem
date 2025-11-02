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
        public string? UserImage { get; set; }

        public required string IdentityUserId { get; set; }
        [ForeignKey(nameof(IdentityUserId))]
        public virtual required IdebtityUser IdentityUser { get; set; }

        public virtual ICollection<Profile>? Profiles { get; set; } = new HashSet<Profile>();
    }
}
