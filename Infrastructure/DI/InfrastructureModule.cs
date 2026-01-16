using Application.Abstractions.CasheStorage.DeviceOTP;
using Application.Abstractions.CasheStorage.Mqtt;
using Application.Abstractions.Identity;
using Application.Abstractions.Image;
using Application.Abstractions.Messaging.mail;
using Application.Abstractions.Security;
using Application.Abstractions.Security.Interfaces;
using Application.Abstractions.Time;
using Domain.RepositotyInterfaces;
using Infrastructure.Identity;
using Infrastructure.Images;
using Infrastructure.Messaging.mail;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Services;
using Infrastructure.Security;
using Infrastructure.Security.ConfigurationOptions;
using Infrastructure.Storage.DeviceOTP;
using Infrastructure.Storage.Mqtt;
using Infrastructure.Time;
using Application.Abstractions.System;
using Infrastructure.Persistence.Services.Background_Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;


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
            var validConnectionString = TryFindValidConnectionString(connectionString);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(validConnectionString));

             // 10- Database Management
            services.AddScoped<IDatabaseManagement, DatabaseManagementService>();

            // Background Services
            services.AddHostedService<ScheduledBackupService>();

            #endregion

            #region 2) register Redis Connection Multiplexer, and cash services
            // read connection string from appsettings.json
            var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";

            // create singleton multiplexer (thread-safe)
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConn));

            // cash services:
            // 1- OtpDevice Checking Redis Store service
            services.AddSingleton<IOtpDeviceCacheStore, OtpDeviceCacheStore>();
            // 2- Mqtt Unit state store service
            services.AddSingleton<IUnitStateStore, UnitStateStore>();

            #endregion

            #region 3) Authentication Options binding
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<RefreshTokenOptions>(configuration.GetSection("Jwt"));
            #endregion

            #region 4) register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            #endregion

            #region 5) infrastructure Services
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

            #region 5) MQTT Hosting Service
            //services.Configure<MqttOptions>(configuration.GetSection("Mqtt"));

            //services.AddSingleton<IManagedMqttClient>(_ =>
            //{
            //    var factory = new MqttFactory();
            //    return factory.CreateManagedMqttClient();
            //});

            //services.AddSingleton<IUnitMessageSerializer, UnitMessageSerializer>();
            //services.AddSingleton<ITopicParser, TopicParser>();
            //services.AddHostedService<MqttHostedService>();
            //services.AddSingleton<DeviceStateListener>();
            //services.AddSingleton<IMqttBus, MqttBusService>();
            //services.AddScoped<IMqttTopicBuilder, MqttTopicBuilder>();
            #endregion

            return services;
        }

        #region Database Startup:
        private static string TryFindValidConnectionString(string originalConnectionString)
        {
            // 1. Try the original first (fastest)
            if (TestConnection(originalConnectionString)) return originalConnectionString;

            // 2. Parse the builder to keep other settings (Catalog, Security, etc.) but change Source
            var builder = new SqlConnectionStringBuilder(originalConnectionString);
            
            // List of common local instances to try
            var candidates = new[] 
            { 
                ".\\SQLEXPRESS",       // Most common for dev
                ".",                   // Default instance
                "(localdb)\\MSSQLLocalDB", // Visual Studio default
                "localhost"            // Docker or local network
            };

            foreach (var server in candidates)
            {
                // specific check: don't retry the one we just failed on
                if (server.Equals(builder.DataSource, StringComparison.OrdinalIgnoreCase)) continue;

                builder.DataSource = server;
                if (TestConnection(builder.ConnectionString))
                {
                    return builder.ConnectionString;
                }
            }

            // 3. Fallback: Return original and let it throw the normal error so user sees what's wrong
            return originalConnectionString;
        }

        private static bool TestConnection(string connectionString)
        {
            try
            {
                // Use a short timeout (e.g., 2 seconds) so startup isn't delayed by 10+ seconds
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = 2
                };

                using var conn = new SqlConnection(builder.ConnectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
