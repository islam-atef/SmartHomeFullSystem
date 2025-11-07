using Application.Entities.SqlEntities.UsersEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddAppIdentityService(this IServiceCollection services)
        {
            services
                .AddIdentity<AppIdentityUser, IdentityRole<Guid>>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<Persistence.AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
