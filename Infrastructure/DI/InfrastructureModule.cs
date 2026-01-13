using Application.Abstractions.CasheStorage.DeviceOTP;
using Application.Abstractions.CasheStorage.Mqtt;
using Application.Abstractions.Identity;
using Application.Abstractions.Image;
using Application.Abstractions.Messaging.mail;
using Application.Abstractions.Messaging.Mqtt;
using Application.Abstractions.Security;
using Application.Abstractions.Security.Interfaces;
using Application.Abstractions.Time;
using Domain.RepositotyInterfaces;
using Infrastructure.Identity;
using Infrastructure.Images;
using Infrastructure.Messaging.mail;
using Infrastructure.Messaging.Mqtt;
using Infrastructure.Messaging.Mqtt.Parsing;
using Infrastructure.Messaging.Mqtt.Serialization;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Infrastructure.Security.ConfigurationOptions;
using Infrastructure.Storage.DeviceOTP;
using Infrastructure.Storage.Mqtt;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
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
            #endregion

            #region 3) Authentication Options binding
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<RefreshTokenOptions>(configuration.GetSection("Jwt"));
            #endregion

            #region 4) register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            #endregion

            #region 5) Redis Cache Store service
            // 1- OtpDevice Checking Redis Store service
            services.AddSingleton<IOtpDeviceCacheStore, OtpDeviceCacheStore>();
            // 2- Mqtt Unit state store service
            services.AddSingleton<IUnitStateStore, UnitStateStore>();

            #endregion

            #region 6) infrastructure Services
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

            #region 7) MQTT Hosting Service
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
    }
}
