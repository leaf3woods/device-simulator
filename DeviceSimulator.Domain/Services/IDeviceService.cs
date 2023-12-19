
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Domain.Services
{
    public interface IDeviceService : IBaseService
    {
        Task<IEnumerable<Device>> GetDevicesAsync(int pageIndex = 0, int pageSize = 0);

        Task<IEnumerable<DeviceType>> GetDeviceTypesAsync();
        Task<int> CreateDevicesAsync(params Device[] devices);

        Task<int> DeleteDevicesAsync(params string[] Uris);

        Task<int> CreateDeviceTypeAsync(DeviceType type);

        Task<int> DeleteDeviceTypesAsync(params string[] typeCodes);

        Task<int> UpdateOrAddDeviceTypesAsync(params DeviceType[] deviceTypes);

        Task SendOnlineAsync(params Device[] devices);

        Task SendJsonMessageAsync<TJsonMessage>(TJsonMessage message, params Device[] devices)
            where TJsonMessage : JsonMessage, IAsJsonNode;

        Task SendBinaryMessageAsync<TBinaryMessage>(TBinaryMessage message, params Device[] devices)
            where TBinaryMessage : BinaryMessage, IAsJsonNode;

        Task SendOfflineAsync(params Device[] devices);

    }
}
