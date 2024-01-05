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

        public async Task LogTraceAsync(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Trace
            };
            await LogWithMeta(meta);
        }

        public async Task LogInformationAsync(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Information
            };
            await LogWithMeta(meta);
        }

        public async Task LogWarningAsync(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Warning
            };
            await LogWithMeta(meta);
        }

        public async Task LogErrorAsync(string message)
        {
            var meta = new MetaLog
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Category = typeof(TCategory).Name,
                Content = message,
                Level = LogLevel.Error
            };
            await LogWithMeta(meta);
        }

        private async Task LogWithMeta(MetaLog meta)
        {
            await Application.Current.Dispatcher.InvokeAsync((Action)(() =>
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
