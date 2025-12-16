using Application.Abstractions.Image;
using Application.Entities.SqlEntities.UsersEntities;
using Application.RepositotyInterfaces;
using Domain.RepositotyInterfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public IAppUserRepo AppUser { get; }
        public IUserRefreshTokenRepo UserRefreshToken { get; }
        public IAppDeviceRepo AppDevice { get; }
        public IDeviceSessionRepo DeviceSession { get; }
        public IAppUserManagementRepo AppUserManagement { get; }
        public IHomeRepo Home {  get; }
        public IHomeSubscriptionRqRepo HomeSubscription { get; }
        public IRoomRepo Room {  get; }

        public EfUnitOfWork(AppDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;

            AppUser = new AppUserRepo(_context);
            UserRefreshToken = new UserRefreshTokenRepo(_context);
            AppDevice = new AppDeviceRepo(_context);
            DeviceSession = new DeviceSessionRepo(_context);
            AppUserManagement = new AppUserManagementRepo(_context, _logger);
            Home = new HomeRepo(_context, _logger);
            Room = new RoomRepo(_context, _logger);
            HomeSubscription = new HomeSubscriptionRqRepo(_context, _logger);
        }
    }
}
