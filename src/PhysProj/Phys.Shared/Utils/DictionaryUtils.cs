namespace Phys.Shared.Utils
{
    public static class DictionaryUtils
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(valueFactory);

            if (dictionary.TryGetValue(key, out var value))
                return value;

            value = valueFactory(key);
            dictionary.Add(key, value);
            return value;
        }
    }
}
