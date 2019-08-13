using System.Collections.Generic;

namespace AntiAntiSwearingBot
{
    public static class IReadOnlyDictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
        {
            TValue res = default(TValue);
            if (key != null)
                dict.TryGetValue(key, out res);
            return res;
        }
    }
}
