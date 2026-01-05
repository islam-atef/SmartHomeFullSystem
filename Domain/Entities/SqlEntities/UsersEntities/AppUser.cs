using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities.SqlEntities.UsersEntities
{
    public class AppUser : BaseEntity<Guid>
    {
        public AppUser() { }

        public string? UserImageUrl { get; set; } = null;

        public Guid IdentityUserId { get; private set; }
        public virtual AppIdentityUser IdentityUser { get; set; } = default!;

        public virtual ICollection<Profile>? Profiles { get; set; } = new HashSet<Profile>(); // (related to the rooms and homes)

        public virtual ICollection<Home>? Homes { get; set; } = new HashSet<Home>();

        public virtual ICollection<AppDevice>? AppDevices { get; set; } = new HashSet<AppDevice>();

        public virtual ICollection<UserRefreshToken>? UserRefreshTokens { get; set; } = new HashSet<UserRefreshToken>();

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
