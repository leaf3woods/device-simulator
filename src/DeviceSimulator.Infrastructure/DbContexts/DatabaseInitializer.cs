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
                //await _logger.LogInformationAsync($"database migrate succeed, with {count} changes");
            }
            catch (Exception ex)
            {
                await context.Database.EnsureCreatedAsync();
                //await _logger.LogErrorAsync($"database initialize failed: {ex}");
            }
        }
    }
}
