using System;
using System.Collections.Generic;
using System.Linq;

namespace Triple.Api
{
    public static class LinqExtensions
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                return;
            }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> defaultValueProvider = null)
        {
            if (dictionary == null)
            {
                return defaultValueProvider != null ? defaultValueProvider() : default(TValue);
            }

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                return defaultValueProvider != null ? defaultValueProvider() : default(TValue);
            }

            return value;
        }

        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Func<TKey, TValue, bool> predicate)
        {
            foreach (var key in dictionary.Keys.ToArray().Where(key => predicate(key, dictionary[key])))
            {
                dictionary.Remove(key);
            }
        }

        public static void AddRange<TTarget>(this ISet<TTarget> set, IEnumerable<TTarget> targets)
        {
            foreach (var target in targets)
            {
                set.Add(target);
            }
        }

        public static IEnumerable<TTarget> AsEnumerable<TTarget>(this TTarget target)
        {
            yield return target;
        }
    }
}
