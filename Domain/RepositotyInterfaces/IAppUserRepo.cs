using Domain.Entities.SqlEntities.UsersEntities;
using Domain.GenericResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IAppUserRepo
    {
        Task<bool> IsUserExistsAsync(Guid id, CancellationToken ct = default);
        Task<GenericResult<AppUser>> AddUserAsync(AppUser user, CancellationToken ct = default);
        Task<GenericResult<AppUser>> UpdateUserAsync(AppUser user, CancellationToken ct = default);
        Task<GenericResult<string?>> AddOrUpdateUserImageAsync(IFormFileCollection imageFile, AppUser user, CancellationToken ct = default);
    }
}
