using FluentValidation;

namespace Phys.Lib.Api.Admin.Api.Auth
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(u => u.UserName).NotNull();
            RuleFor(u => u.Password).NotNull();
        }
    }
}
