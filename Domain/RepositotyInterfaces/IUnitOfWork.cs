using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositotyInterfaces
{
    public interface IUnitOfWork
    {
        public IAppUserRepo AppUserRepo { get; }
        public IUserRefreshTokenRepo UserRefreshTokenRepo { get; }
    }
}
