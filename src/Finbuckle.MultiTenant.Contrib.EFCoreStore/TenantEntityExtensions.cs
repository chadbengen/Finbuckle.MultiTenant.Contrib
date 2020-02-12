using Finbuckle.MultiTenant.Contrib.EFCoreStore;
using Finbuckle.MultiTenant.Contrib.Extensions;

namespace Finbuckle.MultiTenant.Contrib.EFCoreStore
{
    public static class TenantEntityExtensions
    {
        public static bool? GetRequiresTwoFactorAuthentication(this ITenantEntity tenant)
        {
            return tenant.Items.GetRequiresTwoFactorAuthentication();
        }
        public static void SetRequiresTwoFactorAuthentication(this ITenantEntity tenant, bool? value)
        {
            tenant.Items.SetRequiresTwoFactorAuthentication(value);
        }


        public static bool? GetIsActive(this ITenantEntity tenant)
        {
            return tenant.Items.GetIsActive();
        }
        public static void SetIsActive(this ITenantEntity tenant, bool? value)
        {
            tenant.Items.SetIsActive(value);
        }
    }
}