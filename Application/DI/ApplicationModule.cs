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
            //services.AddScoped<IDateTimeProvider, DateTimeProvider>(); ///

            // Example: MediatR handlers (if you use CQRS)
            // services.AddMediatR(typeof(ApplicationModule).Assembly);

            // Example: AutoMapper profiles
            // services.AddAutoMapper(typeof(ApplicationModule).Assembly);

            // Example: validation services
            // services.AddValidatorsFromAssembly(typeof(ApplicationModule).Assembly);

            return services;
        }
    }
}
