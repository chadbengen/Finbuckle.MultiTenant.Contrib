using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.Extensions.Configuration;

namespace Finbuckle.MultiTenant.Contrib.Extensions
{
    public static class ConfigurationSectionExtensions
    {
        public static bool IsMultiTenantEnabled(this IConfigurationSection section)
        {
            return bool.Parse(section[Constants.MultiTenantEnabled]);
        }
    }
}