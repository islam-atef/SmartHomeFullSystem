using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.DTOs
{
    public class UserHomesDTO
    {
        public Guid HomeId { get; set; } = default!;
        public bool IsOwner { get; set; } = default!;

        public string HomeName { get; set; } = default!;
        public string? HomeInfo { get; set; } = default!;

        public double Longitude { get; set; } = default!;
        public double Latitude { get; set; } = default!;

        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
