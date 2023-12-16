using DeviceSimulator.Wpf.Models;
using DeviceSimulator.Wpf.Views;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        #region command binding

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand QuitAppCommand { get; set; } = null!;
        public RelayCommand ConfigureMqttCommand { get; set; } = null!;
        public RelayCommand ConfigureMessageCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceCommand { get; set; } = null!;
        public RelayCommand AddDeviceCommand { get; set; } = null!;
        public RelayCommand AddDeviceTypeCommand { get; set; } = null!;
        public RelayCommand DeleteDeviceTypeCommand { get; set; } = null!;
        public RelayCommand SendMessageCommand { get; set; } = null!;
        public RelayCommand SendOfflineCommand { get; set; } = null!;
        public RelayCommand SendOnlineCommand { get; set; } = null!;

        public MainWindowVM()
        {
            QuitAppCommand = new RelayCommand() { ExecuteAction = QuitApp };
            //SelectCfgPathCommand = new RelayCommand() { ExecuteAction = SelectCfgPath };
            //ResetStateCommand = new RelayCommand() { ExecuteAction = ResetState };
            //WriteInCommand = new RelayCommand() { ExecuteAction = WriteIn };
            //ApnConfigCommand = new RelayCommand() { ExecuteAction = ApnConfig };
            //_controller = new DeviceController();
            //_controller.ReplyArrived += DealReply;
        }

        #endregion command binding


        private static readonly string _yshelp = "13";

        #region property binding

        private string? _selectedCom;

        public string? SelectedCom
        {
            get => _selectedCom;
            set
            {
                _selectedCom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCom"));
            }
        }

        public ObservableCollection<int> SupportedBaudrates { get => new ObservableCollection<int> { 9600, 19200, 115200 }; }

        private int? _selectedBaudrate;

        public int? SelectedBaudrate
        {
            get => _selectedBaudrate;
            set
            {
                _selectedBaudrate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBaudrate"));
            }
        }

        private bool _ifWriteSn = true;

        public bool IfWriteSn
        {
            get => _ifWriteSn;
            set
            {
                _ifWriteSn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfWriteSn"));
            }
        }

        private string? _serialNumber;

        public string? SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                IfSnPass = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SerialNumber"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfSnPass"));
            }
        }

        private bool _ifSnPass = false;

        public bool IfSnPass
        {
            get => _ifSnPass;
            set
            {
                _ifSnPass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfSnPass"));
            }
        }

        private bool _ifUseConfig;

        public bool IfUseConfig
        {
            get => _ifUseConfig;
            set
            {
                _ifUseConfig = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfUseConfig"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfNotUseConfig"));
            }
        }

        public bool IfNotUseConfig => !_ifUseConfig;

        private string? _flashConfigPathPath;

        public string? FlashConfigPath
        {
            get => _flashConfigPathPath;
            set
            {
                _flashConfigPathPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlashConfigPath"));
            }
        }

        private bool _ifWriteHost = true;

        public bool IfWriteHost
        {
            get => _ifWriteHost;
            set
            {
                _ifWriteHost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfWriteHost"));
            }
        }

        private string? _mqttServer;

        public string? MqttServer
        {
            get => _mqttServer;
            set
            {
                _mqttServer = value;
                IfHostPass = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MqttServer"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfHostPass"));
            }
        }

        private int? _mqttPort;

        public int? MqttPort
        {
            get => _mqttPort;
            set
            {
                _mqttPort = value;
                IfHostPass = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MqttPort"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfHostPass"));
            }
        }

        private bool _ifHostPass = false;

        public bool IfHostPass
        {
            get => _ifHostPass;
            set
            {
                _ifHostPass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfHostPass"));
            }
        }

        private bool _ifWriteUserName = true;

        public bool IfWriteUserName
        {
            get => _ifWriteUserName;
            set
            {
                _ifWriteUserName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfWriteUserName"));
            }
        }

        private string? _mqttUserName;

        public string? MqttUserName
        {
            get => _mqttUserName;
            set
            {
                IfUserNamePass = false;
                _mqttUserName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MqttUserName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfUserNamePass"));
            }
        }

        private bool _ifUserNamePass = false;

        public bool IfUserNamePass
        {
            get => _ifUserNamePass;
            set
            {
                _ifUserNamePass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfUserNamePass"));
            }
        }

        private bool _ifWritePassword = true;

        public bool IfWritePassword
        {
            get => _ifWritePassword;
            set
            {
                _ifWritePassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfWritePassword"));
            }
        }

        private string? _mqttPassword;

        public string? MqttPassword
        {
            get => _mqttPassword;
            set
            {
                IfPasswordPass = false;
                _mqttPassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MqttPassword"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfPasswordPass"));
            }
        }

        private bool _ifPasswordPass = false;

        public bool IfPasswordPass
        {
            get => _ifPasswordPass;
            set
            {
                _ifPasswordPass = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfPasswordPass"));
            }
        }

        private bool _ifWriteApn = true;

        public bool IfWriteApn
        {
            get => _ifWriteApn;
            set
            {
                _ifWriteApn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IfWriteApn"));
            }
        }

        private string? _helperInfo = MyVersion.Full;

        public string? HelperInfo
        {
            get => _helperInfo;
            set
            {
                _helperInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelperInfo"));
            }
        }

        private ColorIndex _infocolor;

        public ColorIndex InfoColor
        {
            get => _infocolor;
            set
            {
                _infocolor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InfoColor"));
            }
        }

        #endregion property binding

        #region command method

        public void QuitApp(object o)
        {
            var window = o as System.Windows.Window;
            window?.Close();
        }

        #endregion command method
    }

    public enum ColorIndex
    {
        White,
        Red,
        Green,
    }
}