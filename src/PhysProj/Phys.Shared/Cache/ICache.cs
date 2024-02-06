namespace Phys.Shared.Cache
{
    public interface ICache
    {
        T? Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan ttl);

        void Delete(string key);
    }
}
