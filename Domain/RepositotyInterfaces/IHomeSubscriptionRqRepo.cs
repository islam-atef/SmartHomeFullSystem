using Domain.Entities.SqlEntities.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IHomeSubscriptionRqRepo
    {
        Task<bool> CreateRequestAsync(Guid homeId, Guid userId);

        Task<bool> DeleteRequestAsync(Guid requestId);

        Task<bool> AcceptRequestAsync(Guid homeId, Guid userId);

        Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllHomeRequesstAsync(Guid homeId);
        Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllHomeRequesstAsync(Guid homeId, Expression<Func<HomeSubscriptionRequest, bool>> filters);

        Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllUserRequesstAsync(Guid userId);
        Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllUserRequesstAsync(Guid userId, Expression<Func<HomeSubscriptionRequest, bool>> filters);
    }
}
