using System.Text.RegularExpressions;

namespace Phys.Lib.Core
{
    internal class Code
    {
        public static string FromString(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var code = Regex.Replace(value, @"[^\w]", "-").Replace('_', '-').ToLowerInvariant().Trim();

            return code;
        }
    }
}
