using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DeviceSimulator.Infrastructure.Logger
{
    public class LoggerBox<TCategory> : ILoggerBox<TCategory>
    {

        public LoggerBox(
            LoggerBoxConfiguration boxConfiguration,
            ObservableCollection<MetaLog> logs)
        {
            _configuration = boxConfiguration;
            _logs = logs;
        }

        private LoggerBoxConfiguration _configuration;

        private ObservableCollection<MetaLog> _logs;
        public ObservableCollection<MetaLog> Logs
        {
            get => _logs;
        }

        public void LogInformation(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Information
            };
            Logs.Add(meta);
        }

        public void LogWarning(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Warning
            };
            Logs.Add(meta);
        }

        public void LogError(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Error
            };
            Logs.Add(meta);
        }
    }
}
