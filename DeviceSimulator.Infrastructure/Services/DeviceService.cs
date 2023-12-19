using AutoMapper;
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
            ILogger<DeviceService> logger,
            IMapper mapper)
        {
            _iotDbContext = new Lazy<IotDbContext>(dbContextFactory.CreateDbContext());
            _mqttExplorer = mqttExplorer;
            _logger = logger;
            _mapper = mapper;
        }

        private readonly Lazy<IotDbContext> _iotDbContext;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly ILogger<DeviceService> _logger;
        private readonly IMapper _mapper;

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

        public async Task<IEnumerable<DeviceType>> GetDeviceTypesAsync()
        {
            return await _iotDbContext.Value.DeviceTypes.ToArrayAsync();
        }

        public async Task<int> CreateDevicesAsync(params Device[] devices)
        {
            var uris = devices.Select(d => d.Uri);
            var existUris = await _iotDbContext.Value.Devices
                .Where(d => uris.Contains(d.Uri)).Select(d => d.Uri)
                .ToArrayAsync();
            var targets = devices.Where(d => !existUris.Contains(d.Uri)).ToArray();
            if(devices.Length != targets.Length)
            {
                _logger.LogWarning("some devices exist, skip");
            }
            await _iotDbContext.Value.Devices.AddRangeAsync(targets);
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

        public async Task<int> DeleteDeviceTypesAsync(params string[] typeCodes)
        {
            var deviceTypes = await _iotDbContext.Value.DeviceTypes
                .Where(dt => typeCodes.Contains(dt.Code))
                .ToArrayAsync();
            if(deviceTypes != null)
            {
                _iotDbContext.Value.DeviceTypes.RemoveRange(deviceTypes);
                return await _iotDbContext.Value.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> UpdateOrAddDeviceTypesAsync(params DeviceType[] deviceTypes)
        {
            foreach (var item in deviceTypes)
            {
                var type = await _iotDbContext.Value.DeviceTypes.SingleOrDefaultAsync(dt => dt.Code == item.Code);
                if(type is null)
                {
                    await _iotDbContext.Value.DeviceTypes.AddAsync(item);
                }
                else
                {
                    _mapper.Map(item, type);
                    _iotDbContext.Value.Update(type);
                }
            }
            return await _iotDbContext.Value.SaveChangesAsync();           
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
                var exists = await SelectExistDevice(devices);
                var bytes = Encoding.UTF8.GetBytes(message.Raw!);
                await Parallel.ForEachAsync(exists, async (d, _) =>
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
            var exists = await SelectExistDevice(devices);
            await Parallel.ForEachAsync(exists, async (d, _) =>
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
            var exists = await SelectExistDevice(devices);
            var state = new DeviceState
            {
                Connected = 0,
            };
            var offline = new DeviceStateMsg(state);
            var bytes = Encoding.UTF8.GetBytes(offline.Raw!);
            await Parallel.ForEachAsync(exists, async (d, _) =>
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
            var exists = await SelectExistDevice(devices);
            var state = new DeviceState
            {
                Connected = 1,
            };
            var offline = new DeviceStateMsg(state);
            var bytes = Encoding.UTF8.GetBytes(offline.Raw!);
            await Parallel.ForEachAsync(exists, async (d, _) =>
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

        private async Task<IEnumerable<Device>> SelectExistDevice(params Device[] devices)
        {
            var uris = devices.Select(d => d.Uri).ToArray();
            var exists = await _iotDbContext.Value.Devices
                .Where(d => uris.Contains(d.Uri))
                .ToArrayAsync();
            if (exists.Length == devices.Length)
            {
                _logger.LogInformation("all devices[] are exist");
            }
            if (exists.Length == 0)
            {
                _logger.LogWarning("devices not exist");
            }
            return exists;
        }
    }
}
