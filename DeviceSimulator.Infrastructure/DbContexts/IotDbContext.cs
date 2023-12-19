using CaseExtensions;
using DeviceSimulator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BcsJiaer.Infrastructure.DbContexts
{
    public class IotDbContext : DbContext
    {
        public IotDbContext(DbContextOptions<IotDbContext> options) : base(options)
        {
            //
        }

        #region dbsets

        #region device

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }

        #endregion device

        #endregion dbsets

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Table prefix

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.ClrType.Name.ToSnakeCase());
            }

            #endregion Table prefix

            #region entities initialize

            #region device info

            modelBuilder.Entity<Device>()
                .HasIndex(di => di.Uri)
                .IsUnique();

            #endregion device info

            #region device type

            modelBuilder.Entity<DeviceType>()
                .HasIndex(dt => dt.Code)
                .IsUnique();

            modelBuilder.Entity<DeviceType>()
                .HasData(DeviceType.Seeds);

            modelBuilder.Entity<DeviceType>()
                .HasMany(dt => dt.Devices)
                .WithOne(di => di.DeviceType)
                .HasForeignKey(di => di.DeviceTypeCode)
                .HasPrincipalKey(dt => dt.Code);

            #endregion device type

            #endregion entities initialize
        }
    }
}