using BcsJiaer.Infrastructure.DbContexts;
using DeviceSimulator.Infrastructure.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DeviceSimulator.Infrastructure.DbContexts
{
    public class DatabaseInitializer
    {
        public DatabaseInitializer(
            IDbContextFactory<IotDbContext> dbContextFactory,
            ILoggerBox<DatabaseInitializer> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        private readonly IDbContextFactory<IotDbContext> _dbContextFactory;
        private readonly ILoggerBox<DatabaseInitializer> _logger;

        public async Task Initialize()
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                await context.Database.MigrateAsync();
                var count = await context.SaveChangesAsync();
                _logger.LogInformation($"scope table generate succeed, with {count} new scopes");
            }
            catch (Exception ex)
            {
                await context.Database.EnsureCreatedAsync();
                _logger.LogError($"database initialize failed: {ex}");
            }
        }
    }
}
