﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Finbuckle.MultiTenant.Contrib.Extensions
{
    /// <summary>
    /// Provides strongly typed extensions to get/set <see cref="TenantInfo.Items"/>.
    /// </summary>
    /// <remarks>
    /// This was uncoupled from the <see cref="TenantInfoExtensions"/> so that other implementations
    /// of a <see cref="Dictionary{TKey, TValue}"/> could be used.
    /// </remarks>
    public static class DictionaryExtensions
    {
        public static void Set(this IDictionary<string, object> dictionary, string key, bool? value)
        {
            if (dictionary.ContainsKey(key))
            {
                if (value.HasValue)
                {
                    dictionary[key] = value;
                }
                else
                {
                    dictionary.Remove(key);
                }
            }
            else
            {
                if (value.HasValue)
                {
                    dictionary.Add(key, value);
                }
            }
        }
        public static T SafeGet<T>(this IDictionary<string, object> dictionary, string key)
        {
            var hasResult = dictionary.TryGetValue(key, out var result);

            if (!hasResult) return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }
        public static T UnSafeGet<T>(this IDictionary<string, object> dictionary, string key)
        {
            var hasResult = dictionary.TryGetValue(key, out var result);

            if (!hasResult) throw new KeyNotFoundException($"Could not find key: {key}");

            return (T)Convert.ChangeType(result, typeof(T));
        }
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
          where TValue : class
        {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }

        public static IDictionary<string, TValue> ConvertToCaseInSensitive<TValue>(this IDictionary<string, TValue> dictionary)
        {
            var resultDictionary = new Dictionary<string, TValue>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var (key, value) in dictionary)
            {
                resultDictionary.Add(key, value);
            }

            dictionary = resultDictionary;
            return dictionary;
        }
    }
}