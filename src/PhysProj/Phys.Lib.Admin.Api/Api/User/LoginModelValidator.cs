using FluentValidation;
using Phys.Lib.Admin.Api.Api.User;

namespace Phys.Lib.Admin.Api.Api.User
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
