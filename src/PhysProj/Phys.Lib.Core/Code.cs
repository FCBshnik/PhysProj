using FluentValidation;
using System.Text.RegularExpressions;

namespace Phys.Lib.Core
{
    /// <summary>
    /// Ensures that string value is url and file path representative code
    /// </summary
    public static class Code
    {
        private static readonly Validator validator = new Validator();

        public static string NormalizeAndValidate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException("code can not be empty");

            var code = Regex.Replace(value, @"[^\w]", "-").Replace('_', '-').ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(code))
                throw new ValidationException("code can not be empty");

            validator.ValidateAndThrow(code);
            return code;
        }

        internal class Validator : AbstractValidator<string>
        {
            public Validator()
            {
                RuleFor(u => u)
                    .Must(n => n.Count(char.IsLetter) >= 3)
                    .WithMessage("code must contain at least 3 letters");
                RuleFor(u => u)
                    .Must(n => n.Where(char.IsLetter).All(char.IsLower))
                    .WithMessage("code must contain letters only in lower case");
                RuleFor(u => u)
                    .Must(n => char.IsLetter(n.First()))
                    .WithMessage("code must start with letter");
                RuleFor(u => u)
                    .Must(n => char.IsLetter(n.Last()))
                    .WithMessage("code must end with letter");
                RuleFor(u => u)
                    .Must(u => u.All(c => char.IsLetter(c) || char.IsDigit(c) || c == '-'))
                    .WithMessage("code must contain only letters, digits and hyphens");
                RuleFor(u => u)
                    .Must(u => Enumerable.Range(0, u.Length - 1).All(i => char.IsLetterOrDigit(u[i]) || char.IsLetterOrDigit(u[i + 1])))
                    .WithMessage("code letters and digits can be delimited by only one hyphen");
                RuleFor(u => u).MaximumLength(50);
            }
        }
    }
}
