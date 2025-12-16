using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.DTOs
{
    public class UserHomesDTO
    {
        public Guid HomeId { get; set; }
        public string HomeName { get; set; } = string.Empty;
        public double Longitud { get; set; }
        public double Latitude { get; set; }
    }
}
