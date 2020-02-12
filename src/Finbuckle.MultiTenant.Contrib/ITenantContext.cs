namespace Finbuckle.MultiTenant.Contrib
{
    public interface ITenantContext
    {
        TenantInfo Tenant { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
        string TenantResolutionStrategy { get; }
        void SetTenantId(IHaveTenantId obj);
        string TenantClaimName { get; }
    }
}
