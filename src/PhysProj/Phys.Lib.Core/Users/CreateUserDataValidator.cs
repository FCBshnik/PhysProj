using FluentValidation;

namespace Phys.Lib.Core.Users
{
    internal class CreateUserDataValidator : AbstractValidator<CreateUserData>
    {
        public CreateUserDataValidator(IUsersDb db)
        {
            RuleFor(u => u.Name).NotNull();
            RuleFor(u => u.Name).SetValidator(new UserNameValidator());
            RuleFor(u => u.Password).NotNull();
            RuleFor(u => u.Password).SetValidator(new UserPasswordValidator());
            RuleFor(u => u.Role).NotNull();

            RuleFor(u => u.NameLowerCase)
                .Must(n => db.Find(new UsersQuery { NameLowerCase = n }).Count == 0)
                .WithMessage("user name is already taken");
            RuleFor(u => u.Code)
                .Must(c => db.Find(new UsersQuery { Code = c }).Count == 0)
                .WithMessage("user name is already taken");
        }

        public class UserPasswordValidator : AbstractValidator<string>
        {
            public UserPasswordValidator()
            {
                RuleFor(u => u).MinimumLength(6);
            }
        }

        public class UserNameValidator : AbstractValidator<string>
        {
            public UserNameValidator()
            {
                RuleFor(u => u)
                    .Must(n => n.Count(char.IsLetter) >= 3)
                    .WithMessage("must contain at least 3 characters");
                RuleFor(u => u)
                    .Must(n => char.IsLetter(n.First()))
                    .WithMessage("must start with letter");
                RuleFor(u => u).Must(n => n.Trim().Length == n.Length).WithMessage("must not start or end with space");
                RuleFor(u => u)
                    .Matches(@"^[\w\d\- _]+$")
                    .WithMessage("must contain only letters, digits, whitespace, underscore or hyphen");
                RuleFor(u => u).MaximumLength(20);
            }
        }
    }
}
