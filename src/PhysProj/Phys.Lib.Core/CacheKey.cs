namespace Phys.Lib.Core
{
    internal static class CacheKey
    {
        private const string prefix = "physlib";

        public static string Work(string workCode)
        {
            return $"{prefix}:works:{workCode}";
        }
    }
}
