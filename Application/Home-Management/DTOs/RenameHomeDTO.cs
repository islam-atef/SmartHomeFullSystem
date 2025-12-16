using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Home_Management.DTOs
{
    public class RenameHomeDTO
    {
        public string NewName { get; set; } = string.Empty;
        public Guid HomeId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
