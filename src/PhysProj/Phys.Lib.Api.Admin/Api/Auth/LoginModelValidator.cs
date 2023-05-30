using FluentValidation;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(u => u.UserName).NotNull();
            RuleFor(u => u.Password).NotNull();
        }
    }
}
