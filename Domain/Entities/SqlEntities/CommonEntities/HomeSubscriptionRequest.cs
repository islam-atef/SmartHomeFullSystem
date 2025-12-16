using Application.Entities.SqlEntities;
using Application.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SqlEntities.CommonEntities
{
    public class HomeSubscriptionRequest : BaseEntity<Guid>
    {
        public HomeSubscriptionRequest() { }

        public Guid HomeId { get; set; } 
        public Guid UserId { get; set; }
        public bool RequestState { get; set; } = false;

        public static HomeSubscriptionRequest Create(Guid homeId, Guid userId)
        { 
            var request = new HomeSubscriptionRequest
            { Id = homeId, UserId = userId };
            return request;
        }
    }
}
