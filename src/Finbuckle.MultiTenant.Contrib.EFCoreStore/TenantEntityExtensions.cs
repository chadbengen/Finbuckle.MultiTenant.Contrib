using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.EFCoreStore;
using Finbuckle.MultiTenant.Contrib.Extensions;

namespace Finbuckle.MultiTenant.Contrib.EFCoreStore
{
    public static class TenantEntityExtensions
    {
        public static bool? GetRequiresTwoFactorAuthentication(this ITenantEntity tenant)
        {
            return tenant.Items.TryGetValue(Constants.RequiresTwoFactorAuthentication, out var requires2FA) ? (bool?)requires2FA : null;
        }
        public static void SetRequiresTwoFactorAuthentication(this ITenantEntity tenant, bool? value)
        {
            tenant.Items.Set(Constants.RequiresTwoFactorAuthentication, value);
        }

        public static bool? GetIsActive(this ITenantEntity tenant)
        {
            return tenant.Items.TryGetValue(Constants.IsActive, out var isActive) ? (bool?)isActive : null;
        }
        public static void SetIsActive(this ITenantEntity tenant, bool? value)
        {
            tenant.Items.Set(Constants.IsActive, value);
        }
    }
}