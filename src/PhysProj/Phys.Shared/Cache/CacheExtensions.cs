namespace Phys.Shared.Cache
{
    public static class CacheExtensions
    {
        public static T GetOrAdd<T>(this ICache cache, string key, Func<T> factory, TimeSpan? ttl = null)
        {
            var cached = cache.Get<T>(key);
            if (cached != null)
                return cached;

            var value = factory();
            cache.Set(key, value, ttl);
            return value;
        }
    }
}
