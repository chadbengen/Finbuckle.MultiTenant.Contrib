namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    public class Constants 
    {
        public const string RequiresTwoFactorAuthentication = nameof(RequiresTwoFactorAuthentication);

        public const string IsActive = nameof(IsActive);

        public const string MultiTenantEnabled = nameof(MultiTenantEnabled);

        public const string UseTenantCode = nameof(UseTenantCode);

        public const string TenantClaimName = nameof(TenantClaimName);

        public const string CacheMinutes = nameof(CacheMinutes);
        
        public const string Ignore2faWhileDebugging = nameof(Ignore2faWhileDebugging);

        public const string TenantConfigurationSection = "TenantConfiguration";
    }
}
