using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace DeviceSimulator.Infrastructure.Logger
{
    public class MetaLog : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private TimeOnly _time;
        public TimeOnly Time
        {
            get => _time;
            set
            {
                _time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
            }
        }

        private string _category = null!;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Category)));
            }
        }

        private string _content = null!;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
            }
        }

        /// <summary>
        ///     Information = 2
        ///     Warning = 3
        ///     Error = 4
        /// </summary>
        public LogLevel Level { get; set; }

        public override string ToString()
        {
            return $"{Time}: [{Level:F}] <{Category}> {Content}";
        }

        public readonly static MetaLog Default = new()
        {
            Time = TimeOnly.MinValue,
            Level = LogLevel.Information,
            Category = nameof(MetaLog),
            Content = "Test"
        };
    }
}
