using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.DTOs
{
    public record CreateHomeRoomDTO
    {
        public Guid HomeId { get; set; }
        public Guid OwnerId { get; set; }
        public string RoomName { get; set; } = string.Empty;
    }
}
