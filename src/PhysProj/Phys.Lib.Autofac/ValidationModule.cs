using Autofac;
using FluentValidation;

namespace Phys.Lib.Autofac
{
    public class ValidationModule : Module
    {
        private readonly System.Reflection.Assembly assembly;

        public ValidationModule(System.Reflection.Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(AbstractValidator<>))
                .Where(t => !t.IsAbstract)
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
