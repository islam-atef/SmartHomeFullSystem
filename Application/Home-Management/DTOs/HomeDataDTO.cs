using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.DTOs
{
    public record HomeDataDTO
    {
        public Guid HomeId { get; set; }
        public string HomeName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public IReadOnlyList<HomeRoomsDTO> HomeRooms { get; set; } = new List<HomeRoomsDTO>();
        public IReadOnlyList<string> HomeUsers { get; set; } = new List<string>();
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public record HomeRoomsDTO
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set;} = string.Empty;
    }
}
