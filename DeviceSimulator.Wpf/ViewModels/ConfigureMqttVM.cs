
using DeviceSimulator.Wpf.Views;
using System.ComponentModel;
using System.Windows;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureMqttVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitMqttCommand { get; set; } = null!;
        public RelayCommand ApplySettingsCommand { get; set; } = null!;
        public RelayCommand UseDefaultSettingsCommand { get; set; } = null!;

        #region property binding

        private string _ipAddress = string.Empty;
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpAddress)));
            }
        }

        private int? _port;
        public int? Port
        {
            get => _port;
            set
            {
                _port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
            }
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        private bool _showPassword = false;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                _showPassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowPassword)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextBoxVisibility)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PasswordBoxVisibility)));
            }
        }

        public Visibility TextBoxVisibility
        {
            get => _showPassword ? Visibility.Visible : Visibility.Collapsed;
        }
        public Visibility PasswordBoxVisibility
        {
            get => _showPassword ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool _savePassword;
        public bool SavePassword
        {
            get => _savePassword;
            set
            {
                _savePassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SavePassword)));
            }
        }

        #endregion

        public ConfigureMqttVM()
        {
            QuitMqttCommand = new RelayCommand { ExecuteAction = QuitMqtt };
            ApplySettingsCommand = new RelayCommand { ExecuteAction = ApplySettings };
            UseDefaultSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultSettings };
        }

        public void QuitMqtt(object sender)
        {
            var window = sender as ConfigureMqttWindow;
            window?.Hide();
        }

        public void ApplySettings(object sender)
        {
            var window = sender as ConfigureMqttWindow;
            window?.Hide();
        }
        public void UseDefaultSettings(object sender)
        {
        }
    }
}
