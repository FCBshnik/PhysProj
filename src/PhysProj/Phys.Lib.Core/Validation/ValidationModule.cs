using Autofac;
using FluentValidation;

namespace Phys.Lib.Core.Validation
{
    public class ValidationModule : Module
    {
        private readonly System.Reflection.Assembly assembly;

        public ValidationModule(System.Reflection.Assembly assembly)
        {
            this.assembly = assembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(AbstractValidator<>))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
