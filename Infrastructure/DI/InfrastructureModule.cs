using Domain.RepositotyInterfaces;
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
            // 1️)  read connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // 2️)  register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 3️)  register repositories, unit of work, etc.
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            // ... any other infra services

            return services;
        }
    }
}
