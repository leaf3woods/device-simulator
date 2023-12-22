
using System.Collections.ObjectModel;

namespace DeviceSimulator.Infrastructure.Logger
{
    /// <summary>
    ///     依赖倒置
    /// </summary>
    /// <typeparam name="TCategory"></typeparam>
    public interface ILoggerBox<TCategory>
    {
        ObservableCollection<MetaLog> Logs { get; }

        void LogTrace(string message);

        void LogInformation(string message);

        void LogWarning(string message);

        void LogError(string message);
    }
}
