using Autofac;
using Autofac.Extensions.DependencyInjection;
using DeviceSimulator.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using System.Windows;

namespace DeviceSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = Host.CreateDefaultBuilder(e.Args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
            var app = builder.ConfigureContainer<ContainerBuilder>(options =>
                options.RegisterAssemblyModules(Assembly.GetExecutingAssembly()))
                .Build();
            //  若要注入main window, 需要在app.xaml中删除startup uri, 由host通过依赖注入获取窗口并启动
            var mainWindow = app.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
