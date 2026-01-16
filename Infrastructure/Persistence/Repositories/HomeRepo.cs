using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using Domain.RepositotyInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class HomeRepo(AppDbContext context, ILogger<HomeRepo> logger) : IHomeRepo
    {

        private async Task SaveChangesAsync()
        {
            var result = await context.SaveChangesAsync();
            if (result <= 0)
            {
                logger.LogError("HomeRepo: SaveChangesAsync: a problem occur while saving changes at Database");
                throw new Exception("A problem occurred while saving changes to the database.");
            }
        }

        public async Task<Guid> CreateHomeAsync(
            string name,
            string? info, 
            double latitude, 
            double longitude, 
            string iSO3166_2_lvl4, 
            string country, 
            string state, 
            string address, 
            Guid homeOwner)
        {
            if (String.IsNullOrEmpty(name) || 
                String.IsNullOrEmpty(iSO3166_2_lvl4) ||
                String.IsNullOrEmpty(country) ||
                String.IsNullOrEmpty(state) ||
                String.IsNullOrEmpty(address) ||
                homeOwner == Guid.Empty || 
                double.IsNaN(latitude) || 
                double.IsNaN(longitude))
            {
                logger.LogWarning("HomeRepo: CreateHomeAsync: no data provided!");
                return Guid.Empty;
            }
            try
            {
                var home = Home.Create(name, info, latitude, longitude,iSO3166_2_lvl4,country,state,address, homeOwner);
                await context.Homes.AddAsync(home);
                await SaveChangesAsync();
                return home.Id;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: CreateHomeAsync: {error}", ex.Message);
                return Guid.Empty;
            }
        }

        public async Task<bool> RenameHomeAsync(Guid homeId, string newName)
        {
            if (String.IsNullOrEmpty(newName) || homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: RenameHomeAsync: no data provided!");
                return false;
            }
            try
            {
                var home = await context.Homes.FindAsync(homeId);
                if (home == null)
                {
                    logger.LogError("HomeRepo: RenameHomeAsync: no Home with this Id is Found, id: {id}!",homeId);
                    return false;
                }
                home.Rename(newName, home.HomeOwnerId.ToString());
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: RenameHomeAsync: {error}", ex.Message);
                return false;
            }
        }
        public async Task<bool> SetHomeInfoAsync(Guid homeId, string homeInfo)
        {
            if (String.IsNullOrEmpty(homeInfo) || homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: SetHomeIPAsync: data Missing!, homeId: {x}, new homeInfo: {y}", homeId, homeInfo);
                return false;
            }
            try
            {
                var home = await context.Homes.FindAsync(homeInfo);
                if (home == null)
                {
                    logger.LogError("HomeRepo: SetHomeIPAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }
                home.SetHomeInfo(homeInfo);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: SetHomeIPAsync: {error}", ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateHomeLocationAsync(Guid homeId, double latitude, double longitude)
        {
            if (double.IsNaN(latitude) || double.IsNaN(longitude) || homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: UpdateHomeLocationAsync: data Missing!, homeId: {x}, latitude: {y}, longitude: {z}", homeId, latitude, longitude);
                return false;
            }
            try
            {
                var home = await context.Homes.FindAsync(homeId);
                if (home == null)
                {
                    logger.LogError("HomeRepo: UpdateHomeLocationAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }
                home.SetHomeLocation(latitude,longitude);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: UpdateHomeLocationAsync: {error}", ex.Message);
                return false;
            }
        }
        public async Task<bool> SetHomeOwnerAsync(Guid homeId, Guid userId)
        {
            if (userId == Guid.Empty || homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: SetHomeOwnerAsync: data Missing!, homeId: {x}, UserId: {y}", homeId, userId);
                return false;
            }
            try
            {
                var home = await context.Homes.FindAsync(homeId);
                if (home == null)
                {
                    logger.LogError("HomeRepo: SetHomeOwnerAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }
                home.SetHomeOwner(userId);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: SetHomeOwnerAsync: {error}", ex.Message);
                return false;
            }
        }

        public async Task<bool> AddHomeUserAsync(Guid homeId, Guid userId)
        {
            if (homeId == Guid.Empty || userId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: AddHomeUserAsync: data Missing!, homeId: {x}, user {y}", homeId, userId);
                return false;
            }
            try
            {
                var home = await context.Homes
                    .Where(H => H.Id == homeId)
                    .Include(H => H.AppUsers)
                    .FirstOrDefaultAsync();

                if (home == null)
                {
                    logger.LogError("HomeRepo: AddHomeUserAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }

                var user = new AppUser { Id = userId };
                context.Attach(user);

                home.AddUser(user);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: AddHomeUserAsync: {error}", ex.Message);
                return false;
            }
        }
        public async Task<bool> RemoveHomeUserAsync(Guid homeId, Guid userId)
        {
            if (homeId == Guid.Empty || userId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: RemoveHomeRoomAsync: data Missing!, homeId: {x}, user {y}", homeId, userId);
                return false;
            }
            try
            {
                var home = await context.Homes
                    .Where(H => H.Id == homeId)
                    .Include(H => H.AppUsers)
                    .FirstOrDefaultAsync();

                if (home == null)
                {
                    logger.LogError("HomeRepo: RemoveHomeRoomAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }

                var user = new AppUser { Id = userId };
                context.Attach(user);

                home.RemoveUser(user);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: RemoveHomeRoomAsync: {error}", ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckHomeRomeExistanceAsync(Guid homeId, Guid roomID)
        {
            if (homeId == Guid.Empty || roomID == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: CheckHomeRomeExistanceAsync: data Missing!, homeId: {x}, room {y}", homeId, roomID);
                return false;
            }
            try
            {
                return await context.Homes
                    .Where(h => h.Id == homeId)
                    .SelectMany(h => h.HomeRooms)
                    .AnyAsync(r => r.Id == roomID);
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: CheckHomeRomeExistanceAsync: {error}", ex.Message);
                return false;
            }
        }
        public async Task<bool> CheckHomeUserExistanceAsync(Guid homeId, Guid userID)
        {
            if (homeId == Guid.Empty || userID == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: CheckHomeUserExistanceAsync: data Missing!, homeId: {x}, user {y}", homeId, userID);
                return false;
            }
            try
            {
                return await context.Homes
                    .Where(h => h.Id == homeId)
                    .SelectMany(h => h.AppUsers)
                    .AnyAsync(r => r.Id == userID);
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: CheckHomeUserExistanceAsync: {error}", ex.Message);
                return false;
            }
        }

        public async Task<Home?> GetHomeAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeAsync: no data provided!");
                return null;
            }
            try
            {
               var home = await context.Homes
                    .Where(H => H.Id == homeId)
                    .AsNoTracking()
                    .Include(H => H.AppUsers.Select(u => u.Id))
                    .Include(H => H.HomeRooms.Select(r => new {r.Id,r.RoomName}))
                    
                    .FirstOrDefaultAsync();
                return home;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeAsync: {error}", ex.Message);
                return null;
            }
        }
        public async Task<(double Latitude, double Longitude)?> GetHomeLocationAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeLocationAsync: no data provided!");
                return null;
            }
            try
            {
                var home = await context.Homes
                    .Where(H => H.Id == homeId)
                    .AsNoTracking()
                    .Select(h => new { h.Latitude, h.Longitude })
                    .FirstOrDefaultAsync();

                if (home == null)
                    return null;
                else
                    return ( home.Latitude,home.Longitude); 
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeLocationAsync: {error}", ex.Message);
                return null;
            }
        }
        public async Task<string> GetHomeNameAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeNameAsync: no data provided!");
                return "";
            }
            try
            {
                var home = await context.Homes
                    .Where(H => H.Id == homeId)
                    .AsNoTracking()
                    .Select(h => new{ h.Name, h.HomeReference })     
                    .FirstOrDefaultAsync();

                var name = $"{home!.Name} {home.HomeReference}";
                    return name;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeNameAsync: {error}", ex.Message);
                return "";
            }
        }
        public async Task<Guid> GetHomeOwnerAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeOwnerAsync: no data provided!");
                return Guid.Empty;
            }
            try
            {
                var homeOwner = await context.Homes
                    .Where(H => H.Id == homeId)
                    .AsNoTracking()
                    .Select(h => h.HomeOwnerId)
                    .FirstOrDefaultAsync();

                return homeOwner!;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeOwnerAsync: {error}", ex.Message);
                return Guid.Empty;
            }
        }
        public async Task<IReadOnlyList<Room>?> GetHomeRoomsAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeRoomsAsync: no data provided!");
                return null;
            }
            try
            {
                var homeRooms = await context.Homes
                     .Where(H => H.Id == homeId)
                     .AsNoTracking()
                     .Include(H => H.HomeRooms)
                     .SelectMany(h => h.HomeRooms)
                     .ToListAsync();

                return homeRooms;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeRoomsAsync: {error}", ex.Message);
                return null;
            }
        }
        public async Task<IReadOnlyList<AppUser>?> GetHomeUsersAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: GetHomeUsersAsync: no data provided!");
                return null;
            }
            try
            {
                var homeUsers = await context.Homes
                     .Where(H => H.Id == homeId)
                     .AsNoTracking()
                     .Include(H => H.AppUsers)
                     .SelectMany(h => h.AppUsers)
                     .ToListAsync();

                return homeUsers;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: GetHomeUsersAsync: {error}", ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteHomeAsync(Guid homeId)
        {
            if (homeId == Guid.Empty)
            {
                logger.LogWarning("HomeRepo: DeleteHomeAsync: no data provided!");
                return false;
            }
            try
            {
                var home = await context.Homes.FindAsync(homeId);
                if (home == null)
                {
                    logger.LogError("HomeRepo: DeleteHomeAsync: no Home with this Id is Found, id: {id}!", homeId);
                    return false;
                }
                context.Homes.Remove(home);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical("HomeRepo: DeleteHomeAsync: {error}", ex.Message);
                return false;
            }
        }
    }
}
