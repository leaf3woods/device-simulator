

using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace DeviceSimulator.Infrastructure.Logger
{
    public static class LoggerBoxDIExtension
    {
        public static void AddLoggerBox(this IServiceCollection services, LoggerBoxConfiguration configuration)
        {
            var collection = new ObservableCollection<MetaLog>();
            services.AddSingleton(collection);
            services.AddSingleton(configuration);
            services.AddSingleton(typeof(ILoggerBox<>), typeof(LoggerBox<>));
        }
    }
}
