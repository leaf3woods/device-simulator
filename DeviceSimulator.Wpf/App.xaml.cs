using Autofac;
using Autofac.Extensions.DependencyInjection;
using BcsJiaer.Infrastructure.DbContexts;
using DeviceSimulator.Infrastructure.DbContexts;
using DeviceSimulator.Infrastructure.Logger;
using DeviceSimulator.Wpf.Views;
using Microsoft.EntityFrameworkCore;
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
            builder.ConfigureServices(options =>
            {
                options.AddPooledDbContextFactory<IotDbContext>(options =>
                {
                    options.UseSqlite("Data Source=device_access.db; Mode=ReadWriteCreate;").EnableDetailedErrors();
                });
                options.AddAutoMapper(options => options.AddMaps(Assembly.GetExecutingAssembly()));
                options.AddLoggerBox(new LoggerBoxConfiguration
                {
                    MaxLine = 50
                });
            });
            var app = builder.ConfigureContainer<ContainerBuilder>(options =>
                options.RegisterAssemblyModules(Assembly.GetExecutingAssembly()))
                .Build();
            var init = app.Services.GetRequiredService<DatabaseInitializer>();
            init?.Initialize().Wait();
            //  若要注入main window, 需要在app.xaml中删除startup uri, 由host通过依赖注入获取窗口并启动
            var mainWindow = app.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
