using Domain.Entities.SqlEntities.RoomEntities;
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
    public class HomeConfig : BaseEntityConfig<Home, Guid>
    {
        public override void Configure(EntityTypeBuilder<Home> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasMany(H => H.HomeRooms)
                .WithOne(R => R.Home)
                .HasForeignKey(R => R.HomeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(H => H.AppUsers)
                    .WithMany(AU => AU.Homes)
                    .UsingEntity<Dictionary<string, object>>(
                "HomeUsers",
                r => r.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey("AppUserId")
                ,l => l.HasOne<Home>()
                .WithMany()
                .HasForeignKey("HomeId"))
                    .HasKey("HomeId", "AppUserId");
        }
    }
}
