using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.EFCoreStore
{
    public class PrivateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                var hasPrivateSetter = property?.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;
            }
            return prop;
        }
    }

    public static class IDistributedCacheExtensions
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var serialized = cache.GetString(key);

            if (serialized == null) return default;

            return JsonConvert.DeserializeObject<T>(serialized, _settings);
        }
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var serialized = await cache.GetStringAsync(key);
            if (serialized == null) return default;
            return JsonConvert.DeserializeObject<T>(serialized, _settings);
        }
        public static void Set<T>(this IDistributedCache cache, string key, T item)
        {
            var serialized = JsonConvert.SerializeObject(item);
            cache.SetString(key, serialized);
        }
        public static void Set<T>(this IDistributedCache cache, string key, T item, DistributedCacheEntryOptions options)
        {
            var serialized = JsonConvert.SerializeObject(item);
            cache.SetString(key, serialized, options);
        }
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T item)
        {
            var serialized = JsonConvert.SerializeObject(item);
            return cache.SetStringAsync(key, serialized);
        }
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T item, DistributedCacheEntryOptions options)
        {
            var serialized = JsonConvert.SerializeObject(item);
            return cache.SetStringAsync(key, serialized, options);
        }
    }

}
