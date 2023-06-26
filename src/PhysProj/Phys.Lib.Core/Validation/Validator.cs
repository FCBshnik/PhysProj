using Autofac;
using FluentValidation;

namespace Phys.Lib.Core.Validation
{
    public class Validator : IValidator
    {
        // todo: get rid off autofac interface
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
