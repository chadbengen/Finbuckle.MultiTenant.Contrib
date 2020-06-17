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
        public static string GetConnectionString<TDbContext>(this ITenantEntity tenant)
        {
            var key = typeof(TDbContext).Name;
            return tenant.GetConnectionString(key);
        }
        public static string GetConnectionString(this ITenantEntity tenant, string dbName)
        {
            return tenant.Items.UnSafeGet<string>(dbName);
        }
        public static void SetConnectionString<TDbContext>(this ITenantEntity tenant, string connectionString)
        {
            var key = typeof(TDbContext).Name;
            tenant.SetConnectionString(key, connectionString);
        }
        public static void SetConnectionString(this ITenantEntity tenant, string dbName, string connectionString)
        {
            tenant.Items.Set(dbName, connectionString);
        }
    }
}