using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Utilities;
using DeviceSimulator.Domain.ValueObjects.Message.JsonMsg;
using DeviceSimulator.Infrastructure.Logger;
using DeviceSimulator.Wpf.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class ConfigureMessageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand QuitMessageConfigureCommand { get; set; } = null!;
        public RelayCommand ApplyMessageSettingsCommand { get; set; } = null!;
        public RelayCommand UseDefaultMessageSettingsCommand { get; set; } = null!;

        public ConfigureMessageVM(
            ILoggerBox<ConfigureMessageVM> logger)
        {
            QuitMessageConfigureCommand = new RelayCommand { ExecuteAction = QuitMessageConfigure};
            ApplyMessageSettingsCommand = new RelayCommand { ExecuteAction = ApplyMessageSettings };
            UseDefaultMessageSettingsCommand = new RelayCommand { ExecuteAction = UseDefaultMessageSettings };

            _logger = logger;
        }

        private readonly ILoggerBox<ConfigureMessageVM> _logger;

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

        private string? _textBoxContent;
        public string? TextBoxContent
        {
            get => _textBoxContent;
            set
            {
                _textBoxContent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextBoxContent)));
            }
        }
        #endregion

        public static string TemplateJson = JsonSerializer.Serialize(VitalSign.Default, Options.CustomJsonSerializerOptions);
        public const string Json = "json";
        public const string Binary = "binary";

        public void QuitMessageConfigure(object sender)
        {
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }

        public void ApplyMessageSettings(object sender)
        {
            if (string.IsNullOrEmpty(_textBoxContent))
            {
                _logger.LogWarning("text box is empty");
                return;
            }

            var vital = JsonSerializer.Deserialize<VitalSign>(_textBoxContent, Options.CustomJsonSerializerOptions);
            if (vital is null)
            {
                _logger.LogWarning("json format error");
                return;
            }
            var message = SelectedProtocol switch
            {
                Json => new VitalSignMattressJsonMsg(vital),
                Binary => throw new NotImplementedException(),
                _ => null!
            };
            MainWindowVM.Message = message;
            _logger.LogInformation("message set succeed!");
            var window = sender as ConfigureMessageWindow;
            window?.Hide();
        }
        public void UseDefaultMessageSettings(object sender)
        {
            TextBoxContent = TemplateJson;
        }

    }
}
