using Domain.RepositotyInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class EfUnitOfWork : IUnitOfWork
    {
        public IAppUserRepo AppUserRepo => throw new NotImplementedException();

        public IUserRefreshTokenRepo UserRefreshTokenRepo => throw new NotImplementedException();
    }
}
