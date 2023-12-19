using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using DeviceSimulator.Domain.ValueObjects.Message.JsonMsg;
using DeviceSimulator.Infrastructure.Services;
using DeviceSimulator.Wpf.ViewModels.SubVMs;
using DeviceSimulator.Wpf.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            ConfirureMqttWindow mqttWindow,
            ConfigureDeviceTypeWindow deviceTypeWindow,
            NewDeviceWindow deviceWindow,
            ConfigureMessageWindow messageWindow,
            IDeviceService deviceService,
            IMapper mapper
            )
        {
            _mqttWindow = mqttWindow;
            _deviceWindow = deviceWindow;
            _deviceTypeWindow = deviceTypeWindow;
            _messageWindow = messageWindow;
            _deviceService = deviceService;
            _mapper = mapper;

            QuitAppCommand = new RelayCommand() { ExecuteAction = QuitApp };
            ConfigureMqttCommand = new RelayCommand() { ExecuteAction = ConfigureMqtt };
            ConfigureMessageCommand = new RelayCommand() { ExecuteAction = ConfigureMessage };
            AddDeviceCommand = new RelayCommand() { ExecuteAction = AddDevice };
            DeleteDeviceCommand = new RelayCommand() { ExecuteAction = DeleteDevice };
            ConfigureDeviceTypeCommand = new RelayCommand() { ExecuteAction = ConfigureDeviceType };
            SendMessageCommand = new RelayCommand() { ExecuteAction = SendMessage };
            SendOfflineCommand = new RelayCommand() { ExecuteAction = SendOffline };
            SendOnlineCommand = new RelayCommand() { ExecuteAction = SendOnline };

            DataInitialize();
        }

        private readonly ConfirureMqttWindow _mqttWindow;
        private readonly ConfigureMessageWindow _messageWindow;
        private readonly ConfigureDeviceTypeWindow _deviceTypeWindow;
        private readonly NewDeviceWindow _deviceWindow;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;

        #endregion command binding


        //private static readonly string _yshelp = "13";

        #region property binding

        #endregion property binding

        #region static field

        public static ObservableCollection<DeviceGridVM> Devices { get; set; } = [];

        public static IotMessage? Message { get; set; }

        public static ObservableCollection<DeviceTypeVM> DeviceTypes { get; set; } = [];

        #endregion

        #region command method

        public void QuitApp(object o)
        {
            var window = o as System.Windows.Window;
            window?.Close();
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
            var devices = _mapper.Map<IEnumerable<Device>>(Devices).ToArray();
            var task = Message switch
            {
                VitalSignMattressJsonMsg => _deviceService.SendJsonMessageAsync((Message as VitalSignMattressJsonMsg) ?? throw new ArgumentNullException(), devices),
                _ => throw new ArgumentOutOfRangeException("message type not support")
            };
            await task;
        }

        public async void SendOffline(object o)
        {
            var devices = _mapper.Map<IEnumerable<Device>>(Devices).ToArray();
            await _deviceService.SendOnlineAsync(devices);
        }

        public async void SendOnline(object o)
        {
            var devices = _mapper.Map<IEnumerable<Device>>(Devices).ToArray();
            await _deviceService.SendOfflineAsync(devices);
        }


        private async void DataInitialize()
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
        #endregion command method
    }

    public enum ColorIndex
    {
        White,
        Red,
        Green,
    }
}