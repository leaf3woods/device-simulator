
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

        Task LogTraceAsync(string message);

        Task LogInformationAsync(string message);

        Task LogWarningAsync(string message);

        Task LogErrorAsync(string message);
    }
}
