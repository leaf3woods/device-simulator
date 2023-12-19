using BcsJiaer.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DeviceSimulator.Infrastructure.DbContexts
{
    public class DatabaseInitializer
    {
        public DatabaseInitializer(
            IDbContextFactory<IotDbContext> dbContextFactory,
            ILogger<DatabaseInitializer> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        private readonly IDbContextFactory<IotDbContext> _dbContextFactory;
        private readonly ILogger<DatabaseInitializer> _logger;

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
                _logger.LogError("database initialize failed: {0}", ex);
            }
        }
    }
}
