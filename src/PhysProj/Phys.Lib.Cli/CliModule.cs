using Autofac;

namespace Phys.Lib.Cli
{
    internal class CliModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(ICommand<>))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
