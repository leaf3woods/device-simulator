using DeviceSimulator.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeviceSimulator.Wpf.Views
{
    /// <summary>
    /// ConfigureDeviceTypeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigureDeviceTypeWindow : Window
    {
        public ConfigureDeviceTypeWindow(
            ConfigureDeviceTypeVM deviceTypeVm)
        {
            DataContext = deviceTypeVm;
            InitializeComponent();
        }

        public void WindowHeadDragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
    }
}
