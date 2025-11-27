using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.AuthModel
{
    public class LoginRequestDTO
    {
        [EmailAddress]
        public string? Email { get; set; } = default;
        
        public string? Username { get; set; } = default;

        [Required,PasswordPropertyText]
        public string Password { get; set; } = default!;
    }
}
