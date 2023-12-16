
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Domain.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetDevicesAsync(int pageIndex, int pageSize);
        Task<int> CreateDevicesAsync(params Device[] devices);

        Task<int> DeleteDevicesAsync(params string[] Uris);

        Task<int> CreateDeviceTypeAsync(DeviceType type);

        Task<int> DeleteDeviceTypeAsync(string typeCode);

        Task SendOnlineAsync(params Device[] devices);

        Task SendJsonMessageAsync<TJsonMessage>(TJsonMessage message, params Device[] devices)
            where TJsonMessage : JsonMessage, IAsJsonNode;

        Task SendBinaryMessageAsync<TBinaryMessage>(TBinaryMessage message, params Device[] devices)
            where TBinaryMessage : BinaryMessage, IAsJsonNode;

        Task SendOfflineAsync(params Device[] devices);

    }
}
