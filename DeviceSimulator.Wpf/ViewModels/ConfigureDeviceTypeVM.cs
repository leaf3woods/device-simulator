using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Services;
using DeviceSimulator.Infrastructure.Logger;
using DeviceSimulator.Wpf.ViewModels.SubVMs;
using DeviceSimulator.Wpf.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureDeviceTypeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitDeviceTypeConfigureCommand { get; set; } = null!;
        public RelayCommand AddDeviceTypeCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceTypeCommand { get; set; } = null!;

        public ConfigureDeviceTypeVM(
            IDeviceService deviceService,
            IMapper mapper,
            ILoggerBox<ConfigureDeviceTypeVM> logger)
        {
            QuitDeviceTypeConfigureCommand = new RelayCommand { ExecuteAction = QuitDeviceTypeConfigure };
            AddDeviceTypeCommand = new RelayCommand { ExecuteAction = AddDeviceType };
            DeleteDeviceTypeCommand = new RelayCommand { ExecuteAction = DeleteDeviceType };

            _deviceService = deviceService;
            _mapper = mapper;
            _logger = logger;
        }

        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILoggerBox<ConfigureDeviceTypeVM> _logger;

        #region notify property
        #endregion


        public async void QuitDeviceTypeConfigure(object sender)
        {
            try
            {
                var targets = new List<DeviceType>();
                foreach (var item in MainWindowVM.DeviceTypes)
                {
                    var invalid = string.IsNullOrEmpty(item.Code) || string.IsNullOrEmpty(item.Name);
                    if (invalid)
                    {
                        MainWindowVM.DeviceTypes.Remove(item);
                    }
                    else
                    {
                        var type = _mapper.Map<DeviceType>(item);
                        targets.Add(type);
                    }
                }
                var window = sender as ConfigureDeviceTypeWindow;
                window?.Hide();
                await _deviceService.UpdateOrAddDeviceTypesAsync([.. targets]);
                _logger.LogInformation($"apply device types change succeed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"apply device types change failed {ex}");
            }
        }

        public async void AddDeviceType(object sender)
        {
            // do something
            MainWindowVM.DeviceTypes.Add(new DeviceTypeVM
            {
                IsChecked = false,
            });
            await Task.Delay(3 * 100);
        }
        public async void DeleteDeviceType(object sender)
        {
            try
            {
                var types = MainWindowVM.DeviceTypes
                    .Where(dt => dt.IsChecked);
                var codes = types
                    .Select(dt => dt.Code)
                    .ToArray();
                foreach (var type in types)
                {
                    MainWindowVM.DeviceTypes.Remove(type);
                }
                await _deviceService.DeleteDeviceTypesAsync(codes);
                _logger.LogError($"delete selected device types succeed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"delete selected device types failed {ex}");
            }
        }
    }
}
