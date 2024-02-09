using Microsoft.Extensions.Caching.Memory;

namespace Phys.Shared.Cache
{
    public class PhysMemoryCache : ICache
    {
        private readonly IMemoryCache cache;

        public PhysMemoryCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public void Delete(string key)
        {
            cache.Remove(key);
        }

        public T? Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan? ttl = null)
        {
            if (ttl.HasValue)
                cache.Set(key, value, ttl.Value);
            else
                cache.Set(key, value);
        }
    }
}
