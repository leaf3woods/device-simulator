using DeviceSimulator.Wpf.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DeviceSimulator.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVM MainVm { get; set; } = null!;

        public MainWindow(MainWindowVM mainVm)
        {
            MainVm = mainVm;
            this.DataContext = MainVm;
            InitializeComponent();
        }

        public void WindowHeadDragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
    }
}