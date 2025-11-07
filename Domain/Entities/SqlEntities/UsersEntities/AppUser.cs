using Application.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Entities.SqlEntities.UsersEntities
{
    public class AppUser : BaseEntity<Guid>
    {
        public string? UserImageUrl { get; set; } = null;

        public required Guid IdentityUserId { get; init; }
        public virtual AppIdentityUser IdentityUser { get; init; } = default!;

        public virtual ICollection<Profile>? Profiles { get; set; } = new HashSet<Profile>();

        public virtual ICollection<Home>? Homes { get; set; } = new HashSet<Home>();

        public virtual ICollection<AppDevice>? AppDevices { get; set; } = new HashSet<AppDevice>();

        public static AppUser Create(Guid identityUserId)
        {
            if (identityUserId == Guid.Empty) throw new ArgumentException("Invalid identity id");

            return new AppUser
            {
                Id = Guid.NewGuid(),
                IdentityUserId = identityUserId
            };
        }
    }
}
