using Application.Abstractions.Image;
using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using Domain.GenericResult;
using Domain.RepositotyInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Persistence.Repositories
{
    public class AppUserManagementRepo : IAppUserManagementRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppUserManagementRepo> _logger;
        
        public AppUserManagementRepo(AppDbContext context, ILogger<AppUserManagementRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<bool> IsUserExistsAsync(Guid id, CancellationToken ct = default)
            => await _context.AppUsers.AnyAsync(u => u.Id == id, ct);

        public async Task<string?> GetUserImageUrlAsync(Guid userId)
        {
            // 1) Validate input
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("AppUserManagementRepo: GetUserImageUrlAsync: UserId is empty.");
                return null;
            }
            try
            {
                // get the user homes notracking
                var imageUrl = await _context.AppUsers
                    .Where(au => au.Id == userId)
                    .Select(au => au.UserImageUrl)
                    .SingleOrDefaultAsync();

                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AppUserManagementRepo: GetUserImageUrlAsync: {exception}", ex.Message);
                return null;
            }
        }
        public async Task<bool> AddOrUpdateUserImageUrlAsync(string? imageUrl, Guid userId)
        {
            // 1) Validate input
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("AppUserManagementRepo: AddOrUpdateUserImageUrlAsync: UserId is empty. ImageUrl: {ImageUrl}", imageUrl);
                return false;
            }
            try
            {
                // 2) Bring a tracked user 
                var existingUser = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
                if (existingUser is null)
                {
                    _logger.LogWarning("AppUserManagementRepo: AddOrUpdateUserImageUrlAsync: UserId is not exist in DB. UserId: {userId}", userId);
                    return false;
                }
                // 3) Update entity
                existingUser.UserImageUrl = imageUrl;

                // 4) Save DB changes
                var affected = await _context.SaveChangesAsync();
                if (affected <= 0)
                {
                    _logger.LogError("AppUserManagementRepo: AddOrUpdateUserImageUrlAsync: Error while saving data in DB. savechanges returned value: {affected}", affected);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AppUserManagementRepo: AddOrUpdateUserImageUrlAsync: {exception}", ex.Message);    
                return  false;
            }
        }


        public async Task<List<Home>?> GetUserHomesAsync(Guid userId)
        {
            // 1) Validate input
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("AppUserManagementRepo: GetUserHomesAsync: UserId is empty.");
                return null;
            }
            try
            {
                // get the user homes notracking
                var homes = await _context.AppUsers
                    .Where(au => au.Id == userId)
                    .Include(au => au.Homes!)
                        .ThenInclude(h => h.HomeRooms!.Select(r => new {r.Id, r.RoomName}))
                    .Include(au => au.Homes!)
                        .ThenInclude(h => h.AppUsers!.Select(au => au.Id))
                    .SelectMany(au => au.Homes!)
                    .ToListAsync();

                return homes;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AppUserManagementRepo: GetUserHomesAsync: {exception}", ex.Message);
                return null;
            }
        }
        public async Task<bool> RemoveHomeFromUserAsync(Guid userId, Guid homeId)
        {
            if (userId == Guid.Empty || homeId == Guid.Empty)
            {
                _logger.LogWarning("AppUserManagementRepo: RemoveHomeFromUserAsync: input data are missing. UserId : {userId}, HomeId : {homeId}", userId, homeId);
                return false;
            }
            try
            {
                var user = new AppUser { Id = userId };
                var home = new Home { Id = homeId };

                // attach stubs
                _context.Attach(user);
                _context.Attach(home);

                // Ensure Homes collection is not null before removing
                if (user.Homes != null)
                {
                    user.Homes.Remove(home);
                }
                else
                {
                    _logger.LogWarning("AppUserManagementRepo: RemoveHomeFromUserAsync: User.Homes is null for UserId: {userId}", userId);
                    return false;
                }

                var savingResult = await _context.SaveChangesAsync();
                if (savingResult <= 0)
                {
                    _logger.LogError("AppUserManagementRepo: RemoveHomeFromUserAsync: Error while saving data in DB. savechanges returned value: {savingResult}", savingResult);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AppUserManagementRepo: RemoveHomeFromUserAsync: {exception}", ex.Message);
                return false;
            }
        }


        public async Task<List<Profile>?> GetUserHomeProfilesAsync(Guid userId, Guid homeId)
        {
            // 1) Validate input
            if (userId == Guid.Empty || homeId == Guid.Empty)
            {
                _logger.LogWarning("AppUserManagementRepo: GetUserHomeProfilesAsync: input data are missing. UserId : {userId}, HomeId : {homeId}", userId, homeId);
                return null;
            }
            try
            {
                // get the user's home profiles notracking
                var profiles = await _context.Profiles
                    .Where(p => p.UserId == userId || p.HomeId == homeId )
                    .Include(p => p.RoomProfiles)
                        .ThenInclude(rp => rp.Room)
                    .AsNoTracking()
                    .ToListAsync();

                if (!profiles.Any())
                {
                    _logger.LogWarning("AppUserManagementRepo: GetUserHomeProfilesAsync: there is no profiles for this user in this home");
                    return null;
                }

                return profiles;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AppUserManagementRepo: GetUserHomeProfilesAsync: {exception}", ex.Message);
                return null;
            }
        }

        
    }
}
