using FluentValidation;

namespace Phys.Lib.Core.Users
{
    internal class UserValidator : AbstractValidator<CreateUserData>
    {
        private readonly IUsersDb db;

        public UserValidator(IUsersDb db)
        {
            this.db = db;

            RuleFor(u => u.Name).NotNull();
            RuleFor(u => u.Name)
                .Must(n => n.Count(char.IsLetter) >= 3)
                .WithMessage("must contain at least 3 characters");
            RuleFor(u => u.Name)
                .Must(n => char.IsLetter(n.First()))
                .WithMessage("must start with letter");
            RuleFor(u => u.Name).Must(n => n.Trim().Length == n.Length).WithMessage("must not start or end with space");
            RuleFor(u => u.Name)
                .Matches(@"^[\w\d\- _]+$")
                .WithMessage("must contain only letters, digits, whitespace, underscore or hyphen");
            RuleFor(u => u.Name).MaximumLength(20);
            RuleFor(u => u.Name)
                .Must(n => db.Find(new UsersQuery { NameLowerCase = n.ToLowerInvariant() }).Count == 0)
                .WithMessage("user name is already taken");
            RuleFor(u => u.Name)
                .Must(n => db.Find(new UsersQuery { Code = Code.FromString(n) }).Count == 0)
                .WithMessage("user name is already taken");

            RuleFor(u => u.Password).NotNull();
            RuleFor(u => u.Password).MinimumLength(6);

            RuleFor(u => u.Role).NotNull();
        }
    }
}
