using FluentValidation;

namespace Phys.Lib.Core.Validation
{
    public class Validator : IValidator
    {
        private readonly Func<Type, object> validatorFactory;

        public Validator(Func<Type, object> validatorFactory)
        {
            this.validatorFactory = validatorFactory;
        }

        public void Validate<T>(T value)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(typeof(T));
            ((IValidator<T>)validatorFactory(validatorType)).ValidateAndThrow(value);
        }
    }
}
