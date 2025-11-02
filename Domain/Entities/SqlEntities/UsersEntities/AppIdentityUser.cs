using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.UsersEntities
{
    public class AppIdentityUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
