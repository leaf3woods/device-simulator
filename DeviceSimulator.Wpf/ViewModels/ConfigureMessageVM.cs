using DeviceSimulator.Wpf.Views;
using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureMessageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitMessageCommand { get; set; } = null!;
        public RelayCommand ApplySettingsCommand { get; set; } = null!;
        public RelayCommand UseDefaultSettingsCommand { get; set; } = null!;

        public ConfigureMessageVM()
        {
            QuitMessageCommand = new RelayCommand { ExecuteAction = QuitMessage};
            ApplySettingsCommand = new RelayCommand { ExecuteAction = ApplySettings };
            UseDefaultSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultSettings };
        }

        #region notify property
        #endregion

        public void QuitMessage(object sender)
        {
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }

        public void ApplySettings(object sender)
        {
            // do something
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }
        public void UseDefaultSettings(object sender)
        {
        }

    }
}
