using Domain.Entities.SqlEntities.UsersEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ProfileConfig : BaseEntityConfig<Profile, Guid>
    {
        public override void Configure(EntityTypeBuilder<Profile> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasMany(P => P.RoomProfiles)
                .WithOne(RP => RP.Profile)
                .HasForeignKey(RP => RP.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Home)
                .WithMany()
                .HasForeignKey(p => p.HomeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    } 
}
