using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
        public Guid? OtpQuestionId { get; set; } = Guid.Empty;
    }
}
