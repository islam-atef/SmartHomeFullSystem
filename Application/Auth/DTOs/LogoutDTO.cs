using Domain.GenericResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTOs
{
    public class LogoutDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public String UserEmail { get; set; } = string.Empty;
        public string DeviceMAC { get; set; } = string.Empty;
    }
}
