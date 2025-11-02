using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities
{
    public class BaseEntity<T>
    {
        [Required]
        public required T Id { get; set; } 

        [Required] 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
