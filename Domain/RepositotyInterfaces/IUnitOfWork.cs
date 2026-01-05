using Domain.RepositotyInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IUnitOfWork
    {
        public IAppUserRepo AppUser { get; }
        public IAppUserManagementRepo AppUserManagement { get; }
        public IUserRefreshTokenRepo UserRefreshToken { get; }
        public IAppDeviceRepo AppDevice { get; }
        public IDeviceSessionRepo DeviceSession { get; }
        public IHomeRepo Home { get; }
        public IRoomRepo Room { get; }
        public IHomeSubscriptionRqRepo HomeSubscription { get; }
        
    }
}
