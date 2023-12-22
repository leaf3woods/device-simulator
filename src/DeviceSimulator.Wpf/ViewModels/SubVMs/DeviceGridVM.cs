using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels.SubVMs
{
    public class DeviceGridVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InEditingPayload"));
            }
        }

        private string? _name = string.Empty;
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string _uri = string.Empty;
        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Uri)));
            }
        }

        public string _deviceTypeCode = string.Empty;
        public string DeviceTypeCode
        {
            get => _deviceTypeCode;
            set
            {
                _deviceTypeCode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceTypeCode)));
            }
        }

        public string _version = string.Empty;
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Version)));
            }
        }
    }
}
