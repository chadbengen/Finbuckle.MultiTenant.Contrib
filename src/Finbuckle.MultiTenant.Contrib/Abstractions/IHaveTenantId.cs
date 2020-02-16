namespace Finbuckle.MultiTenant.Contrib.Abstractions
{
    public interface IHaveTenantId
    {
        string TenantId { get; set; }
    }
}
