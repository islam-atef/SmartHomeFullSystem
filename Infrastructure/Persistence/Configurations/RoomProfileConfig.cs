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
    public class RoomProfileConfig : BaseEntityConfig<RoomProfile, Guid>
    {
        public override void Configure(EntityTypeBuilder<RoomProfile> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.HasOne(x => x.LastState)
                .WithOne()
                .HasForeignKey<RoomState>(x => x.RoomProfileId)   
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(typeof(RoomState), nameof(RoomProfile.FavoriteStates))
                .WithOne(nameof(RoomState.RoomProfile))
                .HasForeignKey(nameof(RoomState.RoomProfileId))
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
