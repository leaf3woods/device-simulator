using Autofac;
using DeviceSimulator.Infrastructure.DbContexts;
using DeviceSimulator.Infrastructure.Mqtt;

namespace DeviceSimulator.Wpf.Utillities.AutofacModules
{
    public class SingletonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApiMqttPub>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<DatabaseInitializer>()
                .SingleInstance();
        }
    }
}
