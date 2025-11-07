using Application.Entities.SqlEntities.RoomEntities;
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
    public class RoomConfig : BaseEntityConfig<Room, Guid>
    {
        public override void Configure(EntityTypeBuilder<Room> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasMany(R => R.RoomProfiles)
               .WithOne(RP => RP.Room)
               .HasForeignKey(RP => RP.RoomId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(R => R.RoomControlUnits)
                .WithOne(CU => CU.Room)
                .HasForeignKey(CU => CU.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
