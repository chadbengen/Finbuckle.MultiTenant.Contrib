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
            return tenantInfo.Items.UnSafeGet<bool>(Constants.RequiresTwoFactorAuthentication);
        }
        public static void SetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.Set(Constants.RequiresTwoFactorAuthentication, value);
        }

        public static bool? GetIsActive(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.UnSafeGet<bool>(Constants.IsActive);
        }
        public static void SetIsActive(this TenantInfo tenantInfo, bool? value)
        {
            tenantInfo.Items.Set(Constants.IsActive, value);
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