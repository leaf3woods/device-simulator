using BcsJiaer.Infrastructure.DbContexts;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using DeviceSimulator.Domain.ValueObjects.Message.JsonMsg;
using DeviceSimulator.Infrastructure.Mqtt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DeviceSimulator.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        public DeviceService(
            IDbContextFactory<IotDbContext> dbContextFactory,
            IMqttExplorer mqttExplorer,
            ILogger<DeviceService> logger)
        {
            _iotDbContext = new Lazy<IotDbContext>(dbContextFactory.CreateDbContext());
            _mqttExplorer = mqttExplorer;
            _logger = logger;
        }

        private readonly Lazy<IotDbContext> _iotDbContext;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly ILogger<DeviceService> _logger;

        public async Task<IEnumerable<Device>> GetDevicesAsync(int pageIndex = 0, int pageSize = 0)
        {
            var devices = _iotDbContext.Value.Devices;
            if(pageIndex == 0 || pageSize == 0)
            {
                return await devices.ToArrayAsync();
            }
            else
            {
                return await devices
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            }
        }

        public async Task<int> CreateDevicesAsync(params Device[] devices)
        {
            await _iotDbContext.Value.Devices.AddRangeAsync(devices);
            return await _iotDbContext.Value.SaveChangesAsync();
        }

        public async Task<int> CreateDeviceTypeAsync(DeviceType type)
        {
            await _iotDbContext.Value.DeviceTypes.AddAsync(type);
            return await _iotDbContext.Value.SaveChangesAsync();
        }

        public async Task<int> DeleteDevicesAsync(params string[] uris)
        {
            var devices = await _iotDbContext.Value.Devices
                .Where(d => uris.Contains(d.Uri))
                .ToArrayAsync();
            _iotDbContext.Value.Devices.RemoveRange(devices);
            return await _iotDbContext.Value.SaveChangesAsync();
        }

        public async Task<int> DeleteDeviceTypeAsync(string typeCode)
        {
            var deviceType = await _iotDbContext.Value.DeviceTypes
                .SingleOrDefaultAsync(dt => dt.Code == typeCode);
            if(deviceType != null)
            {
                _iotDbContext.Value.DeviceTypes.Remove(deviceType);
                return await _iotDbContext.Value.SaveChangesAsync();
            }
            return 0;
        }

        public async Task SendJsonMessageAsync<TJsonMessage>(TJsonMessage message, params Device[] devices)
            where TJsonMessage : JsonMessage, IAsJsonNode
        {
            if (message.Raw is null)
            {
                return;
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(message.Raw!);
                await Parallel.ForEachAsync(devices, async (d, _) =>
                {
                    await _mqttExplorer.PublishAsync(
                        IotTopicBuilder.CreateBuilder()
                            .WithDirection(MqttDirection.Up)
                            .WithDeviceType(d.DeviceTypeCode)
                            .WithUri(d.Uri)
                            .WithTag(MqttTag.State)
                                .Build(),
                        bytes);
                });
            }
        }

        public async Task SendBinaryMessageAsync<TBinaryMessage>(TBinaryMessage message, params Device[] devices)
            where TBinaryMessage : BinaryMessage, IAsJsonNode
        {
            await Parallel.ForEachAsync(devices, async (d, _) =>
            {
                await _mqttExplorer.PublishAsync(
                    IotTopicBuilder.CreateBuilder()
                        .WithDirection(MqttDirection.Up)
                        .WithDeviceType(d.DeviceTypeCode)
                        .WithUri(d.Uri)
                        .WithTag(MqttTag.State)
                            .Build(),
                    message.AsFrame());
            });
            _logger.LogInformation($"device send binary message succeed");
        }

        public async Task SendOfflineAsync(params Device[] devices)
        {
            var state = new DeviceState
            {
                Connected = 0,
            };
            var offline = new DeviceStateMsg(state);
            var bytes = Encoding.UTF8.GetBytes(offline.Raw!);
            await Parallel.ForEachAsync(devices, async (d, _) =>
            {
                await _mqttExplorer.PublishAsync(
                    IotTopicBuilder.CreateBuilder()
                        .WithDirection(MqttDirection.Up)
                        .WithDeviceType(d.DeviceTypeCode)
                        .WithUri(d.Uri)
                        .WithTag(MqttTag.State)
                            .Build(),
                    bytes);
            });
            _logger.LogInformation($"device offline succeed");
        }

        public async Task SendOnlineAsync(params Device[] devices)
        {
            var state = new DeviceState
            {
                Connected = 1,
            };
            var offline = new DeviceStateMsg(state);
            var bytes = Encoding.UTF8.GetBytes(offline.Raw!);
            await Parallel.ForEachAsync(devices, async (d, _) =>
            {
                await _mqttExplorer.PublishAsync(
                    IotTopicBuilder.CreateBuilder()
                        .WithDirection(MqttDirection.Up)
                        .WithDeviceType(d.DeviceTypeCode)
                        .WithUri(d.Uri)
                        .WithTag(MqttTag.State)
                            .Build(),
                    bytes);
            });
            _logger.LogInformation($"device online succeed");
        }
    }
}
