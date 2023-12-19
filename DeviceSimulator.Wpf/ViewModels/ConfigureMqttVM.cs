
using DeviceSimulator.Wpf.Views;
using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureMqttVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitMqttCommand { get; set; } = null!;
        public RelayCommand ApplySettingsCommand { get; set; } = null!;
        public RelayCommand UseDefaultSettingsCommand { get; set; } = null!;

        public ConfigureMqttVM()
        {
            QuitMqttCommand = new RelayCommand { ExecuteAction = QuitMqtt };
            ApplySettingsCommand = new RelayCommand { ExecuteAction = ApplySettings };
            UseDefaultSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultSettings };
        }

        public void QuitMqtt(object sender)
        {
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }

        public void ApplySettings(object sender)
        {
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }
        public void UseDefaultSettings(object sender)
        {
        }
    }
}
