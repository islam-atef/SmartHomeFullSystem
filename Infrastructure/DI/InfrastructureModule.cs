using Application.Abstractions.Cashing.interfaces;
using Application.Abstractions.Identity;
using Application.Abstractions.Security.Interfaces;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using Application.RepositotyInterfaces;
using Application.RuleServices;
using Infrastructure.Cashing;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,IConfiguration configuration)
        {
            // 1️)  read connection string from configuration and register DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 2) register Redis Connection Multiplexer
            // read connection string from appsettings.json
            var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            // create singleton multiplexer (thread-safe)
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConn));
            // 1- OtpDeviceCheck
            services.AddScoped<IOtpDeviceCacheStore, OtpDeviceCacheStore>();


            // 3) register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // 4) any other infra services   
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

            return services;
        }
    }
}
