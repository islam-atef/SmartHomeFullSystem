using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.DTOs
{
    public class HomeCardDTO
    {
        public required Guid HomeId { get; set; }

        public required string HomeName { get; set; } = string.Empty;
        public string? HomeInfo { get; set; } = string.Empty;

        public required string OwnerName { get; set; } = string.Empty;

        public required double Longitude { get; set; } = default!;
        public required double Latitude { get; set; } = default!;

        public required string Country { get; set; } = default!;
        public required string State { get; set; } = default!;
        public required string Address { get; set; } = default!;
    }
}
