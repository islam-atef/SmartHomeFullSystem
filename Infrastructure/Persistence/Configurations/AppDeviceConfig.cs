using Application.Entities.SqlEntities.DiviceEntities;
using Application.Entities.SqlEntities.UsersEntities;
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
    public class AppDeviceConfig : BaseEntityConfig<AppDevice, Guid>
    {
        public override void Configure(EntityTypeBuilder<AppDevice> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasMany(AD => AD.RefreshTokens)
                .WithOne()
                .HasForeignKey(RT => RT.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(H => H.DeviceUsers)
                    .WithMany(AU => AU.AppDevices)
                    .UsingEntity<Dictionary<string, object>>(
                "DeviceUsers",
                r => r.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey("AppUserId")
                , l => l.HasOne<AppDevice>()
                .WithMany()
                .HasForeignKey("DeviceId"))
                    .HasKey("DeviceId", "AppUserId");
        }
    }
}
