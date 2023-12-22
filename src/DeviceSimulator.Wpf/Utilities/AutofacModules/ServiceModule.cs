using Autofac;
using DeviceSimulator.Domain.Services;
using System.Reflection;

namespace DeviceSimulator.Wpf.Utilities.AutofacModules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("DeviceSimulator." + nameof(Infrastructure)))
                .Where(type => type.IsAssignableTo(typeof(IBaseService)))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
