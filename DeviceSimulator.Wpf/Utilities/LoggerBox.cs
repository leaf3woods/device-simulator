using DeviceSimulator.Infrastructure.Logger;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace DeviceSimulator.Wpf.Logger
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

        public void LogTrace(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Trace
            };
            LogWithMeta(meta);
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
            LogWithMeta(meta);
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
            LogWithMeta(meta);
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
            LogWithMeta(meta);
        }

        private void LogWithMeta(MetaLog meta)
        {

            Application.Current.Dispatcher.InvokeAsync((Action)(() =>
            {
                if (_logs.Count == _configuration.MaxLine)
                {
                    _logs.RemoveAt(0);
                }
                Logs.Add(meta);
            }));
        }
    }
}
