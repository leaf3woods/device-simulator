
using DeviceSimulator.Infrastructure.Mqtt;
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

        public ConfigureMqttVM(IMqttExplorer mqttExplorer)
        {
            QuitMqttCommand = new RelayCommand { ExecuteAction = QuitMqtt };
            ApplySettingsCommand = new RelayCommand { ExecuteAction = ApplySettings };
            UseDefaultSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultSettings };

            _mqttExplorer = mqttExplorer;
        }
        
        private readonly IMqttExplorer _mqttExplorer;

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

        private string _ipAddressHint = "xxx.xxx.xxx.xxx";
        public string IpAddressHint
        {
            get => _ipAddressHint;
            set
            {
                _ipAddressHint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpAddressHint)));
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

        private string _portHint = "0-65536";
        public string PortHint
        {
            get => _portHint;
            set
            {
                _portHint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PortHint)));
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

        private string _usernameHint = "字母数字下划线";
        public string UsernameHint
        {
            get => _usernameHint;
            set
            {
                _usernameHint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UsernameHint)));
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

        public void QuitMqtt(object? sender)
        {
            var window = sender as ConfigureMqttWindow;
            window?.Hide();
        }

        public void ApplySettings(object? sender)
        {
            var ipPass = string.IsNullOrEmpty(IpAddress);
            var portPass = Port is not null && (Port>=0 && Port<=65536);
            var usernamePass = string.IsNullOrEmpty(Username);

            if (ipPass)
            {
                IpAddress = "无效Ip地址";
            }
            if(portPass)
            {
                PortHint = "无效端口";
            }
            if (usernamePass)
            {
                UsernameHint = "无效用户名";
            }
            if(ipPass && portPass && usernamePass)
            {
                _mqttExplorer.RestartAsync(IpAddress, Port!.Value, Username, Password);
                var window = sender as ConfigureMqttWindow;
                window?.Hide();
            }
        }
        public void UseDefaultSettings(object? sender)
        {
            IpAddress = DefaultIpAddress;
            Port = DefaultPort;
            Username = DefaultUsername;
            Password = DefaultPassword;
        }

        public const string DefaultIpAddress = "localhost";
        public const int DefaultPort = 1883;
        public const string DefaultUsername = "backend";
        public const string DefaultPassword = "5PibfhEhmoNXZcK2";
    }
}
