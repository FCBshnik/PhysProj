using FluentValidation;

namespace Phys.Lib.Core
{
    public class Language
    {
        /// <summary>
        /// English
        /// </summary>
        public static readonly Language En = new Language("en", "English");

        /// <summary>
        /// Russian
        /// </summary>
        public static readonly Language Ru = new Language("ru", "Russian");

        /// <summary>
        /// French
        /// </summary>
        public static readonly Language Fr = new Language("fr", "French");

        /// <summary>
        /// Italian
        /// </summary>
        public static readonly Language It = new Language("it", "Italian");

        /// <summary>
        /// Dutch
        /// </summary>
        public static readonly Language Nl = new Language("nl", "Dutch");

        /// <summary>
        /// Latin
        /// </summary>
        public static readonly Language La = new Language("la", "Latin");

        /// <summary>
        /// Greek, Modern
        /// </summary>
        public static readonly Language Gr = new Language("el", "Greek, Modern");

        /// <summary>
        /// Greek, Ancient
        /// </summary>
        public static readonly Language Grc = new Language("grc", "Greek, Ancient");

        /// <summary>
        /// German
        /// </summary>
        public static readonly Language De = new Language("de", "German");

        public static readonly Language Default = Ru;

        private static readonly Dictionary<string, Language> languages = new List<Language>
        {
            Ru, En, Fr, It, Nl, La, Gr, Grc, De
        }.ToDictionary(l => l.Code, StringComparer.InvariantCultureIgnoreCase);

        public static readonly IReadOnlyCollection<Language> All = languages.Values.ToList();

        public static readonly IReadOnlyCollection<string> AllAsStrings = languages.Values.Select(l => l.Code).ToList();

        /// <summary>
        /// ISO 639-X code of language
        /// </summary>
        public string Code { get; }

        public string Name { get; }

        private Language(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static string NormalizeAndValidate(string value)
        {
            var parsed = Parse(value);
            if (parsed)
                return parsed.Value.Code;

            throw new ValidationException(parsed.Error);
        }

        public static Result<Language> Parse(string value)
        {
            if (languages.TryGetValue(value, out var language))
                return Result.Ok(language);

            return Result.Fail<Language>($"invalid language '{value}'");
        }

        public static implicit operator string(Language language)
        {
            return language.Code;
        }

        public static explicit operator Language(string value)
        {
            if (languages.TryGetValue(value, out var language))
                return language;

            throw new ApplicationException($"invalid language '{value}'");
        }
    }
}
