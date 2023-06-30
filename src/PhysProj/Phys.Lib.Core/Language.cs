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

        private static readonly Dictionary<string, Language> languages = new List<Language>
        {
            Ru, En, Fr, La, Gr, Grc
        }.ToDictionary(l => l.Code, StringComparer.InvariantCultureIgnoreCase);

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
            if (TryParse(value, out var lang))
                return lang.Code;

            throw new ValidationException($"invalid language '{value}'");
        }

        public static bool TryParse(string value, out Language? language)
        {
            return languages.TryGetValue(value, out language);
        }

        public static implicit operator string(Language language)
        {
            return language.Code;
        }
    }
}
