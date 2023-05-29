using Autofac;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<App>().AsSelf().SingleInstance();

            builder.RegisterType<Validator>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterTypes(new[] { typeof(Users.Users) })
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterModule(new ValidationModule(ThisAssembly));
        }
    }
}
