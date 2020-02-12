using Finbuckle.MultiTenant;
using System;
using Finbuckle.MultiTenant.Contrib.Extensions;

namespace Finbuckle.MultiTenant.Contrib.Extensions
{
    /// <summary>
    /// Provides strongly typed methods to get/set common <see cref="TenantInfo.Items"/>.
    /// </summary>
    public static class TenantInfoExtensions
    {
        public static bool? GetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.GetRequiresTwoFactorAuthentication();
        }
        public static void SetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.SetRequiresTwoFactorAuthentication(value);
        }


        public static bool? GetIsActive(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.GetIsActive();
        }
        public static void SetIsActive(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.SetIsActive(value);
        }
    }
}