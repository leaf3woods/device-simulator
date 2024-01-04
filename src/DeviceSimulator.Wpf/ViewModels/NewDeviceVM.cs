
using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Infrastructure.Logger;
using DeviceSimulator.Wpf.ViewModels.SubVMs;
using DeviceSimulator.Wpf.Views;
using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class NewDeviceVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitNewDeviceCommand { get; set; } = null!;
        public RelayCommand ApplyNewDeviceCommand { get; set; } = null!;
        public RelayCommand GenerateUriCommand { get; set; } = null!;

        public NewDeviceVM(
            IDeviceService deviceService,
            IMapper mapper,
            ILoggerBox<NewDeviceVM> logger)
        {
            QuitNewDeviceCommand = new RelayCommand { ExecuteAction = QuitNewDevice };
            ApplyNewDeviceCommand = new RelayCommand { ExecuteAction = ApplyNewDevice };
            GenerateUriCommand = new RelayCommand { ExecuteAction = GenerateUri };

            _deviceService = deviceService;
            _mapper = mapper;
            _logger = logger;
        }

        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILoggerBox<NewDeviceVM> _logger;

        #region property binding

        private string _deviceName = string.Empty;
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceName)));
            }
        }

        private string _deviceVersion = string.Empty;
        public string DeviceVersion
        {
            get => _deviceVersion;
            set
            {
                _deviceVersion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceVersion)));
            }
        }

        private string _deviceUri = string.Empty;
        public string DeviceUri
        {
            get => _deviceUri;
            set
            {
                _deviceUri = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceUri)));
            }
        }

        public bool UriInputEnable
        {
            get => NewDeviceCount switch
            {
                0 or 1 => true,
                _ => false
            };
        }

        private string _deviceUriHint = "可点击右侧按钮生成";
        public string DeviceUriHint
        {
            get => _deviceUriHint;
            set
            {
                _deviceUriHint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceUriHint)));
            }
        }

        private DeviceTypeVM? _selectedDeviceType;
        public DeviceTypeVM? SelectedDeviceType
        {
            get => _selectedDeviceType;
            set
            {
                _selectedDeviceType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDeviceType)));
            }
        }

        private string _deviceTypeHint = "若不存在请添加";
        public string DeviceTypeHint
        {
            get => _deviceTypeHint;
            set
            {
                _deviceTypeHint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceTypeHint)));
            }
        }

        private int _newDeviceCount = 1;

        public int NewDeviceCount
        {
            get => _newDeviceCount;
            set
            {
                _newDeviceCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ApplyCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UriInputEnable)));
                if (!UriInputEnable) DeviceUriHint = "";
                else DeviceUriHint = "可点击右侧按钮生成";
            }
        }


        public string ApplyCount
        {
            get => $"应用[{_newDeviceCount}]";
        }

        #endregion

        public void QuitNewDevice(object? sender)
        {
            var window = sender as NewDeviceWindow;
            window?.Hide();
        }

        public async void ApplyNewDevice(object? sender)
        {
            var typePass = !string.IsNullOrEmpty(SelectedDeviceType?.Code);
            var uriPass = !string.IsNullOrEmpty(DeviceUri);
            if (!typePass)
            {
                DeviceTypeHint = "未选择设备型号";
            }
            if (!uriPass)
            {
                DeviceUriHint = "未填写设备序列号";
            }
            if(!typePass || (!uriPass && UriInputEnable))
            {
                return;
            }
            if(UriInputEnable && MainWindowVM.Devices.Any(d=>d.Uri == DeviceUri))
            {
                _logger.LogWarning("device uri already exist");
                DeviceUriHint = "当前序列号已存在";
                return;
            }
            try
            {
                var targets = new List<Device>();
                for(var i = 0; i<NewDeviceCount; i++)
                {
                    GenerateUri(null);
                    var newDevice = new DeviceGridVM
                    {
                        IsChecked = false,
                        Uri = DeviceUri,
                        Name = DeviceName,
                        DeviceTypeCode = SelectedDeviceType!.Code
                    };
                    MainWindowVM.Devices.Add(newDevice);
                    targets.Add(_mapper.Map<Device>(newDevice));
                }

                var window = sender as NewDeviceWindow;
                window?.Hide();
                var count = await _deviceService.CreateDevicesAsync(targets.ToArray());
                if (count > 0)
                {
                    _logger.LogInformation($"apply ({count}) device to database succeed");
                }
                else
                {
                    _logger.LogWarning($"apply ({count}) device to database succeed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"apply new device failed {ex}");
            }
        }

        public void GenerateUri(object? sender)
        {
            var maxUri = Convert.ToUInt64(MainWindowVM.Devices.MaxBy(d => d.Uri)?.Uri);
            if(maxUri == 0)
            {
                maxUri = 205220410001;
            }
            else
            {
                maxUri++;
            }
            DeviceUri = $"{maxUri}";
            _logger.LogTrace($"device uri generated: {DeviceUri}");
        }
    }
}
