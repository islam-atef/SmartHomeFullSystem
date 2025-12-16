using Application.Entities.SqlEntities.RoomEntities;
using Application.Entities.SqlEntities.UsersEntities;
using Application.GenericResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IAppUserManagementRepo
    {
        Task<string?> GetUserImageUrlAsync(Guid userId);
        Task<bool> AddOrUpdateUserImageUrlAsync(string? imageUrl, Guid userId);

        Task<List<Home>?> GetUserHomesAsync(Guid userId);
        Task<bool> RemoveHomeFromUserAsync(Guid userId, Guid homeId);

        Task<List<Profile>?> GetUserHomeProfilesAsync(Guid userId, Guid homeId);
    }
}
