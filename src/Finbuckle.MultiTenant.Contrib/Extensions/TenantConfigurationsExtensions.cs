using Finbuckle.MultiTenant.Contrib.Configuration;

namespace Finbuckle.MultiTenant.Contrib.Extensions
{
    public static class TenantConfigurationsExtensions
    {
        public static bool IsMultiTenantEnabled(this TenantConfigurations configurations)
        {
            return configurations.Get<bool>(Constants.MultiTenantEnabled);
        }
        public static bool UseTenantCode(this TenantConfigurations configurations)
        {
            return configurations.Get<bool>(Constants.UseTenantCode);
        }
        public static string TenantClaimName(this TenantConfigurations configurations)
        {
            return configurations.Get<string>(Constants.TenantClaimName);
        }
        public static int CacheMinutes(this TenantConfigurations configurations)
        {
            return configurations.Get<int>(Constants.CacheMinutes);
        }
    }
}