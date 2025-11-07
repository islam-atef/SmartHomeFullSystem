using Domain.RepositotyInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositotyInterfaces
{
    public interface IUnitOfWork
    {
        public IAppUserRepo AppUser { get; }
        public IUserRefreshTokenRepo UserRefreshToken { get; }
        public IAppDeviceRepo AppDevice { get; }
        public IDeviceSessionRepo DeviceSession { get; }
    }
}
