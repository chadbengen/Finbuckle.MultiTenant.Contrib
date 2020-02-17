using Finbuckle.MultiTenant.Contrib.Configuration;

namespace Finbuckle.MultiTenant.Contrib.Extensions
{
    /// <summary>
    /// Provides strongly typed methods to get/set common <see cref="TenantInfo.Items"/>.
    /// </summary>
    public static class TenantInfoExtensions
    {
        public static bool? GetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.TryGetValue(Constants.RequiresTwoFactorAuthentication, out var requires2FA) ? (bool?)requires2FA : null;
        }
        public static void SetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.Set(Constants.RequiresTwoFactorAuthentication, value);
        }

        public static bool? GetIsActive(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.TryGetValue(Constants.IsActive, out var isActive) ? (bool?)isActive : null;
        }
        public static void SetIsActive(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.Set(Constants.IsActive, value);
        }
    }
}