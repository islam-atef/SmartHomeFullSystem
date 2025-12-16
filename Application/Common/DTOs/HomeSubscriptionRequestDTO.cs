using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs
{
    public class HomeSubscriptionRequestDTO
    {
        public Guid requestId { get; set; }

        public Guid HomeId { get; set; }
        public string HomeName { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        
        public DateTime RequestDate { get; set; }
    }
}
