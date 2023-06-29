using FluentValidation;

namespace Phys.Lib.Core.Users
{
    internal class UserCreateValidator : AbstractValidator<UserCreate>
    {
        public UserCreateValidator(IUsersDb db)
        {
            RuleFor(u => u.Name).NotNull();
            RuleFor(u => u.Name).SetValidator(new UserNameValidator());
            RuleFor(u => u.Password).NotNull();
            RuleFor(u => u.Password).SetValidator(new UserPasswordValidator());
            RuleFor(u => u.Role).NotNull();

            RuleFor(u => u.Name.ToLowerInvariant())
                .Must(n => db.Find(new UsersQuery { NameLowerCase = n }).Count == 0)
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
                    .WithMessage("username must contain at least 3 letters");
                RuleFor(u => u)
                    .Must(n => char.IsLetter(n.First()))
                    .WithMessage("username must start with letter");
                RuleFor(u => u)
                    .Matches(@"^[\w\d\- _]+$")
                    .WithMessage("username must contain only letters, digits, whitespaces, underscores and hyphens");
                RuleFor(u => u)
                    .Must(n => n.Trim().Length == n.Length)
                    .WithMessage("username must not start or end with space");
                RuleFor(u => u)
                    .MaximumLength(20);
            }
        }
    }
}
