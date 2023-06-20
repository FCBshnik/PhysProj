using Autofac;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Validator>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterTypes(new[] { typeof(Users.Users), typeof(Authors.Authors) })
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterModule(new ValidationModule(ThisAssembly));
        }
    }
}
