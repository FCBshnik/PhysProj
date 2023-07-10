using FluentValidation;

namespace Phys.Lib.Core.Users
{
    internal static class UserValidators
    {
        public static readonly PasswordValidator Password = new PasswordValidator();
        public static readonly NameValidator Name = new NameValidator();

        public class PasswordValidator : AbstractValidator<string>
        {
            public PasswordValidator()
            {
                RuleFor(u => u).MinimumLength(6);
                RuleFor(u => u).MaximumLength(100);
                RuleFor(u => u)
                    .Must(n => !n.Any(char.IsWhiteSpace))
                    .WithMessage("password must not contain spaces");
            }
        }

        public class NameValidator : AbstractValidator<string>
        {
            public NameValidator()
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
