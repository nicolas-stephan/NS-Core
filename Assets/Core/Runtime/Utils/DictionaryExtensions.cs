using System.Collections.Generic;

namespace NS.Core.Utils {
    public static class DictionaryExtensions {
        public static TValue? GetSafe<TKey, TValue>(this IDictionary<TKey, TValue>? dict, TKey key, TValue? defaultValue = default) {
            if (dict == null || !dict.TryGetValue(key, out var value))
                return defaultValue;
            return value;
        }
    }
}