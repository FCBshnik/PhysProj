using Autofac;
using FluentValidation;

namespace Phys.Lib.Core.Validation
{
    public interface IValidation
    {
        void Validate<T>(T value);
    }

    public class Validator : IValidation
    {
        private readonly ILifetimeScope scope;

        public Validator(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public void Validate<T>(T value)
        {
            scope.Resolve<IValidator<T>>().ValidateAndThrow(value);
        }
    }
}
