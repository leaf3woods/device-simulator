using Autofac;
using System.Reflection;

namespace DeviceSimulator.Utillities.AutofacModules
{
    public class VvmModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(a => a.Name.EndsWith("VM") || a.Name.EndsWith("Window"))
                .SingleInstance().AsSelf().PropertiesAutowired();
        }
    }
}