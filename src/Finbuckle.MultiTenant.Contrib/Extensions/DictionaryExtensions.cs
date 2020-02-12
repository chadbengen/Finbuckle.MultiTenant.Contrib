using System.Collections.Generic;

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
        #region RequiresTwoFactorAuthentication

        public const string RequiresTwoFactorAuthentication = nameof(RequiresTwoFactorAuthentication);
        public static bool? GetRequiresTwoFactorAuthentication(this IDictionary<string, object> dictionary)
        {
            return dictionary.TryGetValue(nameof(RequiresTwoFactorAuthentication), out var requires2FA) ? (bool?)requires2FA : null;
        }
        public static void SetRequiresTwoFactorAuthentication(this IDictionary<string, object> dictionary, bool? value)
        {
            dictionary.Set(RequiresTwoFactorAuthentication, value);
        }

        #endregion

        #region IsActive

        public const string IsActive = nameof(IsActive);
        public static bool? GetIsActive(this IDictionary<string, object> dictionary)
        {
            return dictionary.TryGetValue(nameof(IsActive), out var isActive) ? (bool?)isActive : null;
        }
        public static void SetIsActive(this IDictionary<string, object> dictionary, bool? value)
        {
            dictionary.Set(IsActive, value);
        }

        #endregion
 
        private static void Set(this IDictionary<string, object> dictionary, string key, bool? value)
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
    }
}