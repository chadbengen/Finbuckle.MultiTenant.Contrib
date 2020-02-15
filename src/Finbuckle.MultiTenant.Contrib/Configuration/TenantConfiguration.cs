namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    public class TenantConfiguration : ITenantConfiguration
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
