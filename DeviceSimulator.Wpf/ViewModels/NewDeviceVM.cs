
using DeviceSimulator.Wpf.Views;
using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class NewDeviceVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitNewDeviceCommand { get; set; } = null!;
        public RelayCommand ApplyNewDeviceCommand { get; set; } = null!;

        public NewDeviceVM()
        {
            QuitNewDeviceCommand = new RelayCommand { ExecuteAction = QuitNewDevice };
            ApplyNewDeviceCommand = new RelayCommand { ExecuteAction = ApplyNewDevice };
        }

        public void QuitNewDevice(object sender)
        {
            var window = sender as NewDeviceWindow;
            window?.Hide();
        }

        public void ApplyNewDevice(object sender)
        {
            // do something
            var window = sender as NewDeviceWindow;
            window?.Hide();
        }
    }
}
