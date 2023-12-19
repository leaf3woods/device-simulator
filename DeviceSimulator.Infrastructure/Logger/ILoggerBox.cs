﻿
using System.Collections.ObjectModel;

namespace DeviceSimulator.Infrastructure.Logger
{
    public interface ILoggerBox<TCategory>
    {
        ObservableCollection<MetaLog> Logs { get; }
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}