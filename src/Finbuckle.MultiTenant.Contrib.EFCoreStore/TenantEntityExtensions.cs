using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.EfCoreStore;
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
        public static string GetConnectionString<TDbContext>(this TenantInfo tenantInfo)
        {
            var key = typeof(TDbContext).Name;
            return tenantInfo.GetConnectionString(key);
        }
        public static string GetConnectionString(this TenantInfo tenantInfo, string dbName)
        {
            return tenantInfo.Items.UnSafeGet<string>(dbName);
        }
        public static void SetConnectionString<TDbContext>(this TenantInfo tenantInfo, string connectionString)
        {
            var key = typeof(TDbContext).Name;
            tenantInfo.SetConnectionString(key, connectionString);
        }
        public static void SetConnectionString(this TenantInfo tenantInfo, string dbName, string connectionString)
        {
            tenantInfo.Items.Set(dbName, connectionString);
        }
    }
}