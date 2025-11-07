using Application.Entities.SqlEntities.DiviceEntities;
using Application.Entities.SqlEntities.RoomEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ControlUnitConfig : BaseEntityConfig<ControlUnit, Guid>
    {
        public override void Configure(EntityTypeBuilder<ControlUnit> builder)
        {
            // Apply BaseEntity Cinfigurations.
            base.Configure(builder);

            builder.Property(CU => CU.UnitMAC).IsRequired().HasMaxLength(17);
            builder.Property(CU => CU.UnitFamilyType).IsRequired();
            builder.Property(CU => CU.UnitModel).IsRequired();
        }
    }
}
