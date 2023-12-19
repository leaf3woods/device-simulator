using Autofac;
using DeviceSimulator.Domain.Services;
using System.Reflection;

namespace DeviceSimulator.Wpf.Utillities.AutofacModules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var types = Assembly.Load("DeviceSimulator." + nameof(Infrastructure)).GetTypes()
                .Where(type => type.IsAssignableTo(typeof(IBaseService)));
            builder.RegisterAssemblyTypes(Assembly.Load("DeviceSimulator." + nameof(Infrastructure)))
                .Where(type => type.IsAssignableTo(typeof(IBaseService)))
                .AsImplementedInterfaces()
                .PropertiesAutowired();
        }
    }
}
