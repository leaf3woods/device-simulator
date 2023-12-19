using DeviceSimulator.Wpf.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureMessageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitMessageConfigureCommand { get; set; } = null!;
        public RelayCommand ApplyMessageSettingsCommand { get; set; } = null!;
        public RelayCommand UseDefaultMessageSettingsCommand { get; set; } = null!;

        public ConfigureMessageVM()
        {
            QuitMessageConfigureCommand = new RelayCommand { ExecuteAction = QuitMessageConfigure};
            ApplyMessageSettingsCommand = new RelayCommand { ExecuteAction = ApplyMessageSettings };
            UseDefaultMessageSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultMessageSettings };
        }

        #region notify property

        public ObservableCollection<string> Protocols { get; set; } = [Json, Binary];

        private string _selectedProtocol = Json;
        public string SelectedProtocol
        {
            get => _selectedProtocol;
            set
            {
                _selectedProtocol = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProtocol)));
            }
        }

        private string? _rawJson;
        public string? RawJson
        {
            get => _rawJson;
            set
            {
                _rawJson = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawJson)));
            }
        }
        #endregion

        public const string templateJson = "test";
        public const string Json = "json";
        public const string Binary = "binary";

        public void QuitMessageConfigure(object sender)
        {
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }

        public void ApplyMessageSettings(object sender)
        {
            // do something
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }
        public void UseDefaultMessageSettings(object sender)
        {
            RawJson = templateJson;
        }

    }
}
