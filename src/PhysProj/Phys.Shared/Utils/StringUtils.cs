namespace Phys.Shared.Utils
{
    public static class StringUtils
    {
        public static string Join(this IEnumerable<string> values, string separator = ",")
        {
            return string.Join(separator, values);
        }
    }
}
