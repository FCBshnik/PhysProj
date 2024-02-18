namespace Phys.Lib.Core
{
    internal static class CacheKeys
    {
        private const string prefix = "physlib";

        public static string Work(string workCode)
        {
            return $"{prefix}:works:{workCode}";
        }

        public static string Author(string authorCode)
        {
            return $"{prefix}:authors:{authorCode}";
        }
    }
}
