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
    /// ConfigureMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigureMessageWindow : Window
    {
        public ConfigureMessageWindow(ConfigureMessageVM configureMessageVM)
        {
            this.DataContext = configureMessageVM;
            InitializeComponent();
        }
        public void WindowHeadDragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
    }
}
