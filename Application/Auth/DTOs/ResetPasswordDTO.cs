using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class ResetPasswordDTO
    {
        public string UserEmail { get; init; } = default!;
        public string UserPassword { get; init; } = default!;
        public string ResetPWToken { get; init; } = default!;
    }
}
