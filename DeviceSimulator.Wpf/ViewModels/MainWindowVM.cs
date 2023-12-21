using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using DeviceSimulator.Domain.ValueObjects.Message.JsonMsg;
using DeviceSimulator.Infrastructure.Logger;
using DeviceSimulator.Infrastructure.Mqtt;
using DeviceSimulator.Infrastructure.Services;
using DeviceSimulator.Wpf.ViewModels.SubVMs;
using DeviceSimulator.Wpf.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        #region command binding

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand QuitAppCommand { get; set; } = null!;
        public RelayCommand ConfigureMqttCommand { get; set; } = null!;
        public RelayCommand ConfigureMessageCommand { get; set; } = null!;
        public RelayCommand AddDeviceCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceCommand { get; set; } = null!;
        public RelayCommand ConfigureDeviceTypeCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceTypeCommand { get; set; } = null!;
        public RelayCommand SendMessageCommand { get; set; } = null!;
        public RelayCommand SendOfflineCommand { get; set; } = null!;
        public RelayCommand SendOnlineCommand { get; set; } = null!;

        public MainWindowVM(
            ConfigureMqttWindow mqttWindow,
            ConfigureDeviceTypeWindow deviceTypeWindow,
            NewDeviceWindow deviceWindow,
            ConfigureMessageWindow messageWindow,
            IDeviceService deviceService,
            IMapper mapper,
            ILoggerBox<MainWindowVM> logger,
            ObservableCollection<MetaLog> logs,
            IMqttExplorer mqttExplorer
            )
        {
            _mqttWindow = mqttWindow;
            _deviceWindow = deviceWindow;
            _deviceTypeWindow = deviceTypeWindow;
            _messageWindow = messageWindow;
            _deviceService = deviceService;
            _mapper = mapper;
            Logger = logger;
            _logs = logs;
            _mqttExplorer = mqttExplorer;

            QuitAppCommand = new RelayCommand() { ExecuteAction = QuitApp };
            ConfigureMqttCommand = new RelayCommand() { ExecuteAction = ConfigureMqtt };
            ConfigureMessageCommand = new RelayCommand() { ExecuteAction = ConfigureMessage };
            AddDeviceCommand = new RelayCommand() { ExecuteAction = AddDevice };
            DeleteDeviceCommand = new RelayCommand() { ExecuteAction = DeleteDevice };
            ConfigureDeviceTypeCommand = new RelayCommand() { ExecuteAction = ConfigureDeviceType };
            SendMessageCommand = new RelayCommand() { ExecuteAction = SendMessage };
            SendOfflineCommand = new RelayCommand() { ExecuteAction = SendOffline };
            SendOnlineCommand = new RelayCommand() { ExecuteAction = SendOnline };

            DataInitializeAsync();
            StartMqttAsync();
        }

        private readonly ConfigureMqttWindow _mqttWindow;
        private readonly ConfigureMessageWindow _messageWindow;
        private readonly ConfigureDeviceTypeWindow _deviceTypeWindow;
        private readonly NewDeviceWindow _deviceWindow;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IMqttExplorer _mqttExplorer;
        public ILoggerBox<MainWindowVM> Logger;

        #endregion command binding

        #region property binding

        private ObservableCollection<MetaLog> _logs;
        public ObservableCollection<MetaLog> Logs
        {
            get => _logs;
        }

        #endregion property binding

        #region static field

        public static ObservableCollection<DeviceGridVM> Devices { get; set; } = [];

        public static IotMessage? Message { get; set; }

        public static ObservableCollection<DeviceTypeVM> DeviceTypes { get; set; } = [];

        #endregion

        #region command method

        public void QuitApp(object o)
        {
            Application.Current.Shutdown();
        }

        public void ConfigureMqtt(object o)
        {
            _mqttWindow.ShowDialog();
            _mqttWindow.Activate();
        }

        public void ConfigureMessage(object o)
        {
            _messageWindow.ShowDialog();
            _messageWindow.Activate();
        }

        public void AddDevice(object o)
        {
            _deviceWindow.ShowDialog();
            _deviceWindow.Activate();
        }

        public async void DeleteDevice(object o)
        {
            await _deviceService.DeleteDevicesAsync();
        }

        public void ConfigureDeviceType(object o)
        {
            _deviceTypeWindow.ShowDialog();
            _deviceTypeWindow.Activate();
        }

        public async void SendMessage(object o)
        {
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    Logger.LogWarning($"no devices was selected");
                    return;
                }
                var task = Message switch
                {
                    VitalSignMattressJsonMsg => _deviceService.SendJsonMessageAsync((Message as VitalSignMattressJsonMsg) ?? throw new ArgumentNullException(), targets),
                    _ => throw new ArgumentOutOfRangeException("message type not support")
                };
                await task;
                Logger.LogInformation("devices message send succeed");
            }
            catch(Exception ex)
            {
                Logger.LogError($"devices message send failed {ex}");
            }
        }

        public async void SendOffline(object o)
        {
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    Logger.LogWarning($"no devices was selected");
                    return;
                }
                await _deviceService.SendOnlineAsync(targets);
                Logger.LogInformation($"devices({targets.Length}) offline succeed");
            }
            catch(Exception ex)
            {
                Logger.LogError($"devices offline failed {ex}");
            }
        }

        public async void SendOnline(object o)
        {
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    Logger.LogWarning($"no devices was selected");
                    return;
                }
                await _deviceService.SendOfflineAsync(targets);
                Logger.LogInformation($"devices({targets.Length}) online succeed");
            }
            catch (Exception ex)
            {
                Logger.LogError($"devices online failed {ex}");
            }
        }

        #endregion command method

        private async void DataInitializeAsync()
        {
            var devices = (await _deviceService.GetDevicesAsync())
            .Select(d => new DeviceGridVM
            {
                IsChecked = false,
                Name = d.Name,
                Uri = d.Uri,
                DeviceTypeCode = d.DeviceTypeCode,
            });
            foreach (var item in devices)
            {
                Devices.Add(item);
            }

            var deviceTypes = (await _deviceService.GetDeviceTypesAsync())
            .Select(d => new DeviceTypeVM
            {
                IsChecked = false,
                Name = d.Name,
                Code = d.Code
            });
            foreach (var item in deviceTypes)
            {
                DeviceTypes.Add(item);
            }
        }

        private async void StartMqttAsync()
        {
            await _mqttExplorer.StartAsync(
                ConfigureMqttVM.DefaultIpAddress, ConfigureMqttVM.DefaultPort,
                ConfigureMqttVM.DefaultUsername, ConfigureMqttVM.DefaultPassword);
            Logger.LogTrace($"trying connect to mqtt server({ConfigureMqttVM.DefaultIpAddress}:{ConfigureMqttVM.DefaultPort})...");
        }
    }
}