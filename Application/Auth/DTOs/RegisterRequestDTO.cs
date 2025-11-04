using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class RegisterRequestDTO
    {
        public string Email { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string? DisplayName { get; init; }
    }
}
