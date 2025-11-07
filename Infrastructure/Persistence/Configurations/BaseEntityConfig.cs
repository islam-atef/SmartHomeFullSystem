using Application.Entities.SqlEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public abstract class BaseEntityConfig<TEntity, TKey>
    : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
