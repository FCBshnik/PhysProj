using FluentValidation;
using Phys.Lib.Core.Utils;
using System.Text.RegularExpressions;

namespace Phys.Lib.Core
{
    public class Date
    {
        private static Result<Date> parseFail = Result.Fail<Date>("invalid date");
        private static readonly Regex parseRegex = new Regex(@"^(?<prefix>\d+)(?<approximatePower>Y+)?(?<approximateSkew>\d+)?(?<bce>BCE)?$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public string Code { get; }

        public int Min { get; }

        public int Max { get; }

        private Date(string code, int min, int max)
        {
            Code = code;
            Min = min;
            Max = max;
        }

        public static string NormalizeAndValidate(string? value)
        {
            var parsed = Parse(value);
            if (!parsed)
                throw new ValidationException(parsed.Error);

            return parsed.Value.Code;
        }

        public static void ValidateBornAndDied(string? born, string? died)
        {
            if (!born.HasValue() || !died.HasValue())
                return;

            var bornDate = Parse(born).Value;
            var diedDate = Parse(died).Value;
            ValidateBornAndDied(bornDate, diedDate);
        }

        public static void ValidateBornAndDied(Date born, Date died)
        {
            if (born == null) throw new ArgumentNullException(nameof(born));
            if (died == null) throw new ArgumentNullException(nameof(died));

            if (born.Max > died.Max || died.Min < born.Min)
                throw new ValidationException($"invalid born and died dates");

            if (born.Max < died.Min && died.Min - born.Max >= 100)
                throw new ValidationException($"invalid born and died dates");
        }

        public static Result<Date> Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return parseFail;

            value = value.ToUpperInvariant();

            var match = parseRegex.Match(value);
            if (!match.Success)
                return parseFail;

            var isBeforeCommonEra = match.Groups["bce"].Value.Length > 0;
            var prefix = int.Parse(match.Groups["prefix"].Value);
            var approximatePower = match.Groups["approximatePower"].Value.Length;
            if (approximatePower > 3)
                return parseFail;
            var approximateSkewBaseRaw = match.Groups["approximateSkew"].Value;
            if (approximateSkewBaseRaw.Length > 3)
                return parseFail;

            var approximateSkewBase = (approximateSkewBaseRaw.Length > 0 ? int.Parse(approximateSkewBaseRaw) : 1) * 10;

            if (isBeforeCommonEra)
                prefix *= -1;

            var approximateOrder = (int)Math.Pow(10, approximatePower);
            var approximateSkew = approximatePower > 0 ? (int)Math.Pow(approximateSkewBase, approximatePower) : 0;
            var min = prefix * approximateOrder - approximateSkew;
            var max = prefix * approximateOrder + approximateSkew;

            if (min > 2023)
                return parseFail;

            return Result.Ok(new Date(value, min, max));
        }
    }
}
