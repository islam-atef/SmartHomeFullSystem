using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.CommonEntities;
using Domain.RepositotyInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class HomeSubscriptionRqRepo(AppDbContext context, ILogger<HomeSubscriptionRqRepo> logger) : IHomeSubscriptionRqRepo
    {
        public async Task<bool> CreateRequestAsync(Guid homeId, Guid userId)
        {
            if(homeId == Guid.Empty || userId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: CreateRequestAsync: Missing Data!: homeId = {x}, userId = {y}", homeId, userId);
                return false;
            }
            try
            {
                var request = HomeSubscriptionRequest.Create(homeId, userId);

                await context.HomeSubscriptionRequests.AddAsync(request);
                var result = await context.SaveChangesAsync();
                if(result > 0)
                    return true;

                logger.LogError("HomeSubscriptionRqRepo: CreateRequestAsync: Error while saving new request at the Database!");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: CreateRequestAsync: {e}", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRequestAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: DeleteRequestAsync: Missing Data!: requestId = {x}!", requestId);
                return false;
            }
            try
            {
                var request = await context.HomeSubscriptionRequests.FindAsync(requestId);
                if(request == null)
                {
                    logger.LogWarning("HomeSubscriptionRqRepo: DeleteRequestAsync: there is no request with this ID:{x}!",requestId);
                    return false;
                }
                context.HomeSubscriptionRequests.Remove(request);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return true;

                logger.LogError("HomeSubscriptionRqRepo: DeleteRequestAsync: Error while saving new request at the Database!");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: DeleteRequestAsync: {e}", ex.Message);
                return false;
            }
        }

        public async Task<bool> AcceptRequestAsync(Guid homeId, Guid userId)
        {
            if (homeId == Guid.Empty || userId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: AcceptRequestAsync: Missing Data!: homeId = {x}, userId = {y}", homeId, userId);
                return false;
            }
            try
            {
                var request = await context.HomeSubscriptionRequests
                    .AsTracking()
                    .Where(r => r.HomeId == homeId && r.UserId == userId)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    logger.LogWarning("HomeSubscriptionRqRepo: AcceptRequestAsync: No request exist with these data!");
                    return false;
                }

                request.RequestState = true;

                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return true;
                logger.LogError("HomeSubscriptionRqRepo: AcceptRequestAsync: Error while saving Changes at the Database!");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: AcceptRequestAsync: {e}", ex.Message);
                return false;
            }
        }

        public async Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllHomeRequesstAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: GetAllHomeRequesstAsync: Missing Data!: homeId = {x}", homeId);
                return null;
            }
            try
            {
                var requests = await context.HomeSubscriptionRequests
                    .AsNoTracking()
                    .Where(r => r.HomeId == homeId)
                    .ToListAsync();

                return requests;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: GetAllHomeRequesstAsync: {e}", ex.Message);
                return null;
            }
        }
        public async Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllHomeRequesstAsync(Guid homeId, Expression<Func<HomeSubscriptionRequest, bool>> filters)
        {

            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: GetAllHomeRequesstAsync: Missing Data!: homeId = {x}", homeId);
                return null;
            }
            try
            {
                var requests = await context.HomeSubscriptionRequests
                    .AsNoTracking()
                    .Where(r => r.HomeId == homeId)
                    .Where(filters)
                    .ToListAsync();

                return requests;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: GetAllHomeRequesstAsync: {e}", ex.Message);
                return null;
            }
        }

        public async Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllUserRequesstAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: GetAllUserRequesstAsync: Missing Data!: userId = {x}", userId);
                return null;
            }
            try
            {
                var requests = await context.HomeSubscriptionRequests
                    .AsNoTracking()
                    .Where(r => r.UserId == userId)
                    .ToListAsync();

                return requests;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: GetAllUserRequesstAsync: {e}", ex.Message);
                return null;
            }
        }
        public async Task<IReadOnlyList<HomeSubscriptionRequest>?> GetAllUserRequesstAsync(Guid userId, Expression<Func<HomeSubscriptionRequest, bool>> filters)
        {
            if (userId == Guid.Empty)
            {
                logger.LogWarning("HomeSubscriptionRqRepo: GetAllUserRequesstAsync: Missing Data!: userId = {x}", userId);
                return null;
            }
            try
            {
                var requests = await context.HomeSubscriptionRequests
                    .AsNoTracking()
                    .Where(r => r.UserId == userId)
                    .Where(filters)
                    .ToListAsync();

                return requests;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeSubscriptionRqRepo: GetAllUserRequesstAsync: {e}", ex.Message);
                return null;
            }
        }
    }
}
