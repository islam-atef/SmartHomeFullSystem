using Domain.Entities.SqlEntities.DiviceEntities;
using Domain.Entities.SqlEntities.RoomEntities;
using Domain.Entities.SqlEntities.SecurityEntities;
using Domain.Entities.SqlEntities.UsersEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppIdentityRole : IdentityRole<Guid> { }

    public class AppDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Profile> ProfileUsers { get; set; }

        public virtual DbSet<Home> Homes { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomProfile> RoomProfiles { get; set; }
        public virtual DbSet<RoomState> RoomStates {  get; set; }

        public virtual DbSet<ControlUnit> ControlUnits { get; set; }

        public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // will apply all configurations that implement the IEntityConfiguration<T> in the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
