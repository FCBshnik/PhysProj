using Autofac;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<App>().AsSelf().SingleInstance();

            builder.RegisterTypes(new[] { typeof(Users.Users) })
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            RegisterValidators(builder);
        }

        private void RegisterValidators(ContainerBuilder builder)
        {
            builder.RegisterTypes(new[] { typeof(UserValidator) })
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
        }
    }
}
