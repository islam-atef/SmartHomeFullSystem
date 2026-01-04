using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Identity.DTOs
{
    public class UserIdentityDTO
    {
        public Guid Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? DisplayName { get; set; } = default!;

        public string? Errors { get; set; } = null;
    }
}
