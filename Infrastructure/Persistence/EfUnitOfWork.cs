using Application.Abstractions.Image;
using Application.Entities.SqlEntities.UsersEntities;
using Application.RepositotyInterfaces;
using Domain.RepositotyInterfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
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
        private readonly IImageService _imageService;

        public IAppUserRepo AppUser { get; }
        public IUserRefreshTokenRepo UserRefreshToken { get; }
        public IAppDeviceRepo AppDevice { get; }
        public IDeviceSessionRepo DeviceSession { get; }

        public EfUnitOfWork(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;

            AppUser = new AppUserRepo(_context, _imageService);
            UserRefreshToken = new UserRefreshTokenRepo(_context);
            AppDevice = new AppDeviceRepo(_context);
            DeviceSession = new DeviceSessionRepo(_context);
        }
    }
}
