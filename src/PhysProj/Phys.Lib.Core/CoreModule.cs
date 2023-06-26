using Autofac;
using Phys.Lib.Core.Validation;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Phys.Lib.Tests.Unit")]

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
