using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Domain.ValueObjects.Message;
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
using System.Text;
using System.Windows;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        #region command binding

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand QuitAppCommand { get; set; } = null!;
        public RelayCommand MinimizeAppCommand { get; set; } = null!;
        public RelayCommand ConfigureMqttCommand { get; set; } = null!;
        public RelayCommand ConfigureMessageCommand { get; set; } = null!;
        public RelayCommand AddDeviceCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceCommand { get; set; } = null!;
        public RelayCommand ConfigureDeviceTypeCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceTypeCommand { get; set; } = null!;
        public RelayCommand SendMessageCommand { get; set; } = null!;
        public RelayCommand StopSendMessageCommand { get; set; } = null!;

        public RelayCommand SendOfflineCommand { get; set; } = null!;
        public RelayCommand SendOnlineCommand { get; set; } = null!;

        public MainWindowVM(
            ConfigureMqttWindow mqttWindow,
            ConfigureDeviceTypeWindow deviceTypeWindow,
            NewDeviceWindow deviceWindow,
            NewDeviceVM newDeviceVM,
            ConfigureMessageWindow messageWindow,
            ConfigureMessageVM messageVM,
            IDeviceService deviceService,
            IMapper mapper,
            ILoggerBox<MainWindowVM> logger,
            ObservableCollection<MetaLog> logs,
            IMqttExplorer mqttExplorer
            )
        {
            _mqttWindow = mqttWindow;
            _deviceWindow = deviceWindow;
            _deviceVM = newDeviceVM;
            _deviceTypeWindow = deviceTypeWindow;
            _messageWindow = messageWindow;
            _messageVM = messageVM;
            _deviceService = deviceService;
            _mapper = mapper;
            Logger = logger;
            _logs = logs;
            _mqttExplorer = mqttExplorer;

            QuitAppCommand = new RelayCommand() { ExecuteAction = QuitApp };
            MinimizeAppCommand = new RelayCommand() { ExecuteAction = MinimizeApp };
            ConfigureMqttCommand = new RelayCommand() { ExecuteAction = ConfigureMqtt };
            ConfigureMessageCommand = new RelayCommand() { ExecuteAction = ConfigureMessage };
            AddDeviceCommand = new RelayCommand() { ExecuteAction = AddDevice };
            DeleteDeviceCommand = new RelayCommand() { ExecuteAction = DeleteDevice };
            ConfigureDeviceTypeCommand = new RelayCommand() { ExecuteAction = ConfigureDeviceType };
            SendMessageCommand = new RelayCommand() { ExecuteAction = SendMessage };
            StopSendMessageCommand = new RelayCommand() { ExecuteAction = StopSendMessage };
            SendOfflineCommand = new RelayCommand() { ExecuteAction = SendOffline };
            SendOnlineCommand = new RelayCommand() { ExecuteAction = SendOnline };

            DataInitializeAsync();
            StartMqttAsync();
        }

        private static readonly object _lockObj = new ();

        private readonly ConfigureMqttWindow _mqttWindow;
        private readonly ConfigureMessageWindow _messageWindow;
        private readonly ConfigureMessageVM _messageVM;
        private readonly ConfigureDeviceTypeWindow _deviceTypeWindow;
        private readonly NewDeviceWindow _deviceWindow;
        private readonly NewDeviceVM _deviceVM;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IMqttExplorer _mqttExplorer;
        public ILoggerBox<MainWindowVM> Logger;

        #endregion command binding

        #region property binding

        private bool _isAllItemsChecked;

        public bool IsAllItemsChecked
        {
            get => _isAllItemsChecked;
            set
            {
                _isAllItemsChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAllItemsChecked)));
                Parallel.ForEach(Devices, d =>
                {
                    d.IsChecked = value;
                });
            }
        }

        private int _selectedNewDevicesCount = 1;
        public int SelectedNewDevicesCount
        {
            get => _selectedNewDevicesCount;
            set
            {
                _selectedNewDevicesCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNewDevicesCount)));
            }
        }

        private bool _autoSend;

        public bool AutoSend
        {
            get => _autoSend;
            set
            {
                _autoSend = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoSend)));
            }
        }

        private bool _cancelSend;
        public bool CancelSend
        {
            get => _cancelSend;
            set
            {
                _cancelSend = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableSendButton)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableStopButton)));
            }
        }
        public bool EnableSendButton
        {
            get => !AutoSend || _cancelSend;
        }

        public bool EnableStopButton
        {
            get => AutoSend && !_cancelSend;
        }

        private ObservableCollection<MetaLog> _logs;
        public ObservableCollection<MetaLog> Logs
        {
            get => _logs;
        }

        private StringBuilder _messageHistoryBuilder = new();

        public string MessageHistory
        {
            get => _messageHistoryBuilder.ToString();
        }

        public StringBuilder MessageHistoryBuilder
        {
            get
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MessageHistory)));
                return _messageHistoryBuilder;
            }
        }

        #endregion property binding

        #region static field

        public static ObservableCollection<DeviceGridVM> Devices { get; set; } = [];

        public static IotMessage? Message { get; set; }

        public static ObservableCollection<DeviceTypeVM> DeviceTypes { get; set; } = [];

        public static ObservableCollection<int> NewDevicesCountOption { get; } = [1, 5, 10];

        #endregion

        private int _lastMessageLength;

        #region command method

        public void QuitApp(object? sender)
        {
            Application.Current.Shutdown();
        }

        public void MinimizeApp(object? sender)
        {
            var mainWindow = sender as MainWindow;
            mainWindow!.WindowState = WindowState.Minimized;
        }

        public void ConfigureMqtt(object? sender)
        {
            _mqttWindow.ShowDialog();
            _mqttWindow.Activate();
        }

        public void ConfigureMessage(object? sender)
        {
            _messageWindow.ShowDialog();
            _messageWindow.Activate();
        }

        public void AddDevice(object? sender)
        {
            _deviceVM.NewDeviceCount = SelectedNewDevicesCount;
            _deviceWindow.ShowDialog();
            _deviceWindow.Activate();
        }

        public async void DeleteDevice(object? sender)
        {
            await _deviceService.DeleteDevicesAsync();
        }

        public void ConfigureDeviceType(object? sender)
        {
            _deviceTypeWindow.ShowDialog();
            _deviceTypeWindow.Activate();
        }

        public async void SendMessage(object? sender)
        {
            CancelSend = false;
            //AppendNewHistoryMsg(0, ConfigureMessageVM.TemplateJson);
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    await Logger.LogWarningAsync($"no devices was selected");
                    CancelSend = true;
                    return;
                }
                if (_messageVM.EnableRandom)
                {
                    while (AutoSend && !_cancelSend)
                    {
                        await Parallel.ForEachAsync(targets, async (target, _) =>
                        {
                            var delay = Random.Shared.Next(_messageVM.RandomSeconds, _messageVM.RandomSeconds + 3 * 60);
                            await Logger.LogInformationAsync($"send message in interval: {delay}s");
                            var json = new VitalSignMattressJsonMsg(new VitalSign
                            {
                                Heart = Random.Shared.Next(0, 90),
                                Breath = Random.Shared.Next(0, 50),
                                Move = Random.Shared.Next(0, 1),
                                State = (Random.Shared.Next(0, 100) / 10) switch
                                {
                                    0 or 1 or 2 => (float)MattressState.Leave,
                                    3 => (float)MattressState.WeakSignal,
                                    _ => (float)MattressState.On
                                }
                            });
                            while (delay-- >= 0 && !_cancelSend)
                            {
                                await _deviceService.SendJsonMessageAsync(json, target);
                                //await Logger.LogInformationAsync($"device[{target.Uri}] message send succeed");
                                AppendNewHistoryMsg(1, json.Raw, false);
                                await Task.Delay(1000, _);
                            }
                        });
                    }
                    return;
                }
                switch (Message)
                {
                    case VitalSignMattressJsonMsg:
                        var json = (VitalSignMattressJsonMsg)Message;
                        while (AutoSend && !_cancelSend)
                        {
                            await _deviceService.SendJsonMessageAsync(json, targets);
                            await Logger.LogInformationAsync("devices message send succeed");
                            AppendNewHistoryMsg(targets.Length, json.Raw);
                            await Task.Delay(1000);
                        }
                        break;
                    case VitalSignMattressBinMsg:
                        var bin = (VitalSignMattressBinMsg)Message!;
                        while (AutoSend && _cancelSend)
                        {
                            await _deviceService.SendBinaryMessageAsync(bin, targets);
                            await Logger.LogInformationAsync("devices message send succeed");
                            var plain = Convert.ToHexString(bin.FrameData);
                            AppendNewHistoryMsg(targets.Length, plain);
                            await Task.Delay(1000);
                        }
                        break;
                    default: throw new ArgumentOutOfRangeException("message type not support");
                }
            }
            catch(Exception ex)
            {
                await Logger.LogErrorAsync($"devices message send failed {ex}");
                CancelSend = true;
            }
        }

        public void StopSendMessage(object? sender)
        {
            CancelSend = true;
        }

        public async void SendOffline(object? sender)
        {
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    await Logger.LogWarningAsync($"no devices was selected");
                    return;
                }
                await _deviceService.SendOfflineAsync(targets);
                await Logger.LogInformationAsync($"devices({targets.Length}) offline succeed");
            }
            catch(Exception ex)
            {
                await Logger.LogErrorAsync($"devices offline failed {ex}");
            }
        }

        public async void SendOnline(object? sender)
        {
            try
            {
                var devices = Devices.Where(d => d.IsChecked);
                var targets = _mapper.Map<IEnumerable<Device>>(devices).ToArray();
                if (targets is null || targets.Length == 0)
                {
                    await Logger.LogWarningAsync($"no devices was selected");
                    return;
                }
                await _deviceService.SendOnlineAsync(targets);
                await Logger.LogInformationAsync($"devices({targets.Length}) online succeed");
            }
            catch (Exception ex)
            {
                await Logger.LogErrorAsync($"devices online failed {ex}");
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
            await Logger.LogTraceAsync($"trying connect to mqtt server({ConfigureMqttVM.DefaultIpAddress}:{ConfigureMqttVM.DefaultPort})...");
        }

        private void AppendNewHistoryMsg(int count, string? msg, bool remove = true)
        {
            lock (_lockObj)
            {
                if (_lastMessageLength != 0 && remove)
                {
                    MessageHistoryBuilder.Remove(MessageHistoryBuilder.Length - _lastMessageLength, _lastMessageLength);
                }
                MessageHistoryBuilder.AppendLine($"{TimeOnly.FromDateTime(DateTime.Now):T} [{count}] devs:");
                MessageHistoryBuilder.AppendLine(msg);
                MessageHistoryBuilder.Append(string.Empty);
                _lastMessageLength = msg?.Length + 2 ?? 0;
            }
        }
    }
}