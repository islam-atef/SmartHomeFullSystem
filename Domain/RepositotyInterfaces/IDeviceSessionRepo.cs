using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.GenericResult;
using Domain.Entities.SqlEntities.UsersEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IDeviceSessionRepo
    {
        Task<GenericResult<AppDevice>> AssignNewTokenToDeviceAsync(string deviceMACAddress, UserRefreshToken token);
        Task<GenericResult<AppDevice>> RemoveTokenFromDeviceAsync(string deviceMACAddress, Guid tokenId);

        Task<GenericResult<List<UserRefreshToken>>> GetAllTokensOfDeviceAsync(string deviceMACAddress);
        Task<GenericResult<List<UserRefreshToken>>> GetAllActiveTokensOfDeviceAsync(string deviceMACAddress);
    }
}
