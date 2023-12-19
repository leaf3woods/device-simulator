using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
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
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InEditingPayload"));
                    }
                }
            }
        }

        private string? _name = string.Empty;
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
                    }
                }
            }
        }

        private string _uri = string.Empty;
        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Uri"));
                    }
                }
            }
        }

        public string _deviceTypeCode = string.Empty;
        public string DeviceTypeCode
        {
            get => _deviceTypeCode;
            set
            {
                _deviceTypeCode = value;
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DeviceTypeCode"));
                    }
                }
            }
        }

        public string _version = string.Empty;
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Version"));
                    }
                }
            }
        }
    }
}
