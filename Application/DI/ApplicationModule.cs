using Application.Abstractions.Time;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DI
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            #region Register your application-layer services here
            // 1- Auth Service
            services.AddScoped<IAuthService, AuthService>();
            // 2- Device Checking Service
            services.AddScoped<IDeviceManagementService, DeviceManagementService>();

            #endregion

            return services;
        }
    }
}
