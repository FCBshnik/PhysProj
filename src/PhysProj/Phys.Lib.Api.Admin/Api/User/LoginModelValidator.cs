using FluentValidation;

namespace Phys.Lib.Api.Admin.Api.User
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(u => u.Username).NotNull();
            RuleFor(u => u.Password).NotNull();
        }
    }
}
