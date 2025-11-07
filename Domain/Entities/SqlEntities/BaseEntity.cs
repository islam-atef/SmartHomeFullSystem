using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities.SqlEntities
{
    public abstract class BaseEntity<T>
    {
        public required T Id { get; set; }

        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        public string? CreatedBy { get; protected set; }

        public DateTime? ModifiedAt { get; protected set; }

        public string? ModifiedBy { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public DateTime? DeletedAt { get; protected set; }

        public byte[]? RowVersion { get; protected set; }

        public void MarkDeleted(string? user = null)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            ModifiedBy = user;
        }

        public void UpdateAudit(string? user = null)
        {
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = user;
        }
    }
}
