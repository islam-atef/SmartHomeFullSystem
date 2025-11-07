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
            // Register your application-layer services here
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
