
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

        

        #endregion

        public void QuitNewDevice(object sender)
        {
            var window = sender as NewDeviceWindow;
            window?.Hide();
        }

        public async void ApplyNewDevice(object sender)
        {
            if(SelectedDeviceType is null)
            {
                _logger.LogWarning("device type was not selected");
            }
            try
            {
                var newDevice = new DeviceGridVM
                {
                    IsChecked = false,
                    Uri = DeviceUri,
                    Name = DeviceName,
                    DeviceTypeCode = SelectedDeviceType!.Code
                };
                MainWindowVM.Devices.Add(newDevice);
                var target = _mapper.Map<Device>(newDevice);
                var window = sender as NewDeviceWindow;
                window?.Hide();
                var count = await _deviceService.CreateDevicesAsync(target);
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

        public void GenerateUri(object sender)
        {
            var maxUri = Convert.ToUInt64(MainWindowVM.Devices.MaxBy(d => d.Uri)?.Uri);
            var uri = (ulong)0;
            if(maxUri == 0)
            {
                uri = 205220410001;
            }
            else
            {
                uri = maxUri++;
            }
            DeviceUri = $"{uri}";
            _logger.LogTrace($"device uri generated: {DeviceUri}");
        }
    }
}
