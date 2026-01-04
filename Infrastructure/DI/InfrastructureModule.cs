using Application.Abstractions.CasheStorage.DeviceOTP;
using Application.Abstractions.Identity;
using Application.Abstractions.Image;
using Application.Abstractions.Messaging.mail;
using Application.Abstractions.Security;
using Application.Abstractions.Security.Interfaces;
using Application.Abstractions.Time;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using Application.RepositotyInterfaces;
using Infrastructure.Identity;
using Infrastructure.Images;
using Infrastructure.Messaging.mail;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Infrastructure.Security.ConfigurationOptions;
using Infrastructure.Storage.DeviceOTP;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DI
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            #region 1️) Register DbContext with SQL Server
            // read connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            // Register AppDbContext with the correct options
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion


            #region 2) register Redis Connection Multiplexer
            // read connection string from appsettings.json
            var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            // create singleton multiplexer (thread-safe)
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConn));
            // Redis Cache Store service
            // 1- OtpDevice Checking Redis Store service
            services.AddScoped<IOtpDeviceCacheStore, OtpDeviceCacheStore>();
            #endregion

            #region 3) Options binding
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<RefreshTokenOptions>(configuration.GetSection("Jwt"));
            #endregion


            #region 4) register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            #endregion


            #region 5) any other infra services   
            // 1- Identity configuration
            services.AddAppIdentityService();
            // 2- Identity Management
            services.AddScoped<IIdentityManagement, IdentityManagement>();
            // 3- JWT Token Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            // 4- Custom (Refresh) Token Service
            services.AddScoped<ICustomTokenService, CustomTokenService>();
            // 5- Hashing Service
            services.AddScoped<IHashingService, HashingService>();
            // 6- TimeProvider Service 
            services.AddSingleton(TimeProvider.System);
            // 7- DateTimeProvider Service
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            // 8- Email Service
            services.AddScoped<IEmailService, EmailService>();
            // 9- Image Service
            services.AddScoped<IImageService, ImageService>();
            #endregion

            return services;
        }
    }
}
