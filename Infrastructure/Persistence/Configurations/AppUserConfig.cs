using Application.Entities.SqlEntities.UsersEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class AppUserConfig : BaseEntityConfig<AppUser, Guid>
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasOne(x => x.IdentityUser)
                .WithOne() // no navigation on principal
                .HasForeignKey<AppUser>(x => x.IdentityUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(A => A.UserRefreshTokens)
                .WithOne()
                .HasForeignKey(RT => RT.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);


            // enforce uniqueness of the FK
            builder.HasIndex(x => x.IdentityUserId).IsUnique();

            builder.HasMany(AU => AU.Profiles)
                .WithOne(P => P.User)
                .HasForeignKey(P => P.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
