using Application.Abstractions.Identity;
using Domain.RepositotyInterfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            //  register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // ... any other infra services   
            // 1- Identity configuration
            services.AddAppIdentityService();
            // 2- Identity Management
            services.AddScoped<IIdentityManagement, IdentityManagement>();
            // 3- Auth

            return services;
        }
    }
}
