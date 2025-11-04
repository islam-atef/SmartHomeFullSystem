using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class AccountActivationDTO
    {
        public string UserEmail { get; init; } = default!;
        public string ActivationToken { get; init; } = default!;
    }
}
