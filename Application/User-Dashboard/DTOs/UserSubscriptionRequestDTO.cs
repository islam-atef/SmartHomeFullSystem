using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.DTOs
{
    public class UserSubscriptionRequestDTO
    {
        public Guid RequestId { get; set; }
        public Guid HomeId { get; set; }
        public string HomeName { get; set; } = string.Empty;
        public bool RequestState { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
