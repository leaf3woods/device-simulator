using AutoMapper;
using DeviceSimulator.Domain.Entities;
using DeviceSimulator.Domain.Services;
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
            IMapper mapper)
        {
            QuitDeviceTypeConfigureCommand = new RelayCommand { ExecuteAction = QuitDeviceTypeConfigure };
            AddDeviceTypeCommand = new RelayCommand { ExecuteAction = AddDeviceType };
            DeleteDeviceTypeCommand = new RelayCommand { ExecuteAction = DeleteDeviceType };

            _deviceService = deviceService;
            _mapper = mapper;
        }

        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;

        #region notify property
        #endregion


        public async void QuitDeviceTypeConfigure(object sender)
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
            await _deviceService.UpdateOrAddDeviceTypesAsync([..targets]);
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
        }
    }
}
