using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Application.DI
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Register your application-layer services here
    
            // Example: MediatR handlers (if you use CQRS)
            // services.AddMediatR(typeof(ApplicationModule).Assembly);
    
            // Example: AutoMapper profiles
            // services.AddAutoMapper(typeof(ApplicationModule).Assembly);
    
            // Example: validation services
            // services.AddValidatorsFromAssembly(typeof(ApplicationModule).Assembly);
    
            // Example: your own services/interfaces
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IProfileService, ProfileService>();
            //services.AddScoped<IRoomService, RoomService>();
    
            return services;
        }
    }
}
