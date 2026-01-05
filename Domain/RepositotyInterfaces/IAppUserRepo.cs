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

        Task<GenericResult<(string email, string userName)>> GetUserIdentityInfoAsync(Guid userId, CancellationToken ct = default);
        Task<GenericResult<(string email, string userName, string displayName, string phone)>> GetUserInfoAsync(Guid userId, CancellationToken ct = default);

        Task<GenericResult<bool>> ChangeUserDisplayNameAsync(Guid userId, string newName, CancellationToken ct = default);
        Task<GenericResult<bool>> ChangeUserPhoneNumberAsync(Guid userId, string newNumber, CancellationToken ct = default);
        Task<GenericResult<bool>> ChangeUserNameAsync(Guid userId, string newUserName, CancellationToken ct = default);

        Task<GenericResult<bool>> RemoveUserAsync(Guid userId, CancellationToken ct = default);
    }
}
