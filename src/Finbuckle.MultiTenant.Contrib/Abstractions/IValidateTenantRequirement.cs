namespace Finbuckle.MultiTenant.Contrib.Abstractions
{
    public interface IValidateTenantRequirement
    {
        bool TenantIsRequired();
    }
}
