using DeviceSimulator.Domain.Services;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using DeviceSimulator.Domain.ValueObjects.Message.JsonMsg;
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
        public RelayCommand AddDeviceTypeCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceTypeCommand { get; set; } = null!;
        public RelayCommand SendMessageCommand { get; set; } = null!;
        public RelayCommand SendOfflineCommand { get; set; } = null!;
        public RelayCommand SendOnlineCommand { get; set; } = null!;

        public MainWindowVM(
            ConfirureMqttWindow mqttWindow,
            NewDeviceWindow deviceWindow,
            ConfigureMessageWindow messageWindow,
            IDeviceService deviceService
            )
        {
            _mqttWindow = mqttWindow;
            _deviceWindow = deviceWindow;
            _messageWindow = messageWindow;
            _deviceService = deviceService;

            QuitAppCommand = new RelayCommand() { ExecuteAction = QuitApp };
            ConfigureMqttCommand = new RelayCommand() { ExecuteAction = ConfigureMqtt };
            ConfigureMessageCommand = new RelayCommand() { ExecuteAction = ConfigureMessage };
            AddDeviceCommand = new RelayCommand() { ExecuteAction = AddDevice };
            DeleteDeviceCommand = new RelayCommand() { ExecuteAction = DeleteDevice };
            AddDeviceTypeCommand = new RelayCommand() { ExecuteAction = AddDeviceType };
            DeleteDeviceTypeCommand = new RelayCommand() { ExecuteAction = DeleteDeviceType };
            SendMessageCommand = new RelayCommand() { ExecuteAction = SendMessage };
            SendOfflineCommand = new RelayCommand() { ExecuteAction = SendOffline };
            SendOnlineCommand = new RelayCommand() { ExecuteAction = SendOnline };

            var devices = deviceService.GetDevicesAsync().Result
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
        }

        private readonly ConfirureMqttWindow _mqttWindow;
        private readonly ConfigureMessageWindow _messageWindow;
        private readonly NewDeviceWindow _deviceWindow;
        private readonly IDeviceService _deviceService;

        #endregion command binding


        private static readonly string _yshelp = "13";

        #region property binding


        public static ObservableCollection<DeviceGridVM> Devices { get; set; } = null!;
        public static IotMessage? Message { get; set; }

        #endregion property binding

        #region command method

        public void QuitApp(object o)
        {
            var window = o as System.Windows.Window;
            window?.Close();
        }

        public void ConfigureMqtt(object o)
        {
            _mqttWindow.Show();
            _mqttWindow.Activate();
        }

        public void ConfigureMessage(object o)
        {
            _messageWindow.Show();
            _messageWindow.Activate();
        }

        public void AddDevice(object o)
        {
            _deviceWindow.Show();
            _deviceWindow.Activate();
        }

        public async void DeleteDevice(object o)
        {
            await _deviceService.DeleteDevicesAsync();
        }

        public void AddDeviceType(object o)
        {
            //await _deviceService.CreateDeviceTypeAsync();
        }

        public void DeleteDeviceType(object o)
        {
            //await _deviceService.DeleteDeviceTypeAsync();
        }

        public async void SendMessage(object o)
        {
            var task = Message switch
            {
                VitalSignMattressJsonMsg => _deviceService.SendJsonMessageAsync((Message as VitalSignMattressJsonMsg) ?? throw new ArgumentNullException()),
                _ => throw new ArgumentOutOfRangeException("message type not support")
            };
            await task;
        }

        public async void SendOffline(object o)
        {
            await _deviceService.SendOnlineAsync();
        }

        public async void SendOnline(object o)
        {
            await _deviceService.SendOfflineAsync();
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