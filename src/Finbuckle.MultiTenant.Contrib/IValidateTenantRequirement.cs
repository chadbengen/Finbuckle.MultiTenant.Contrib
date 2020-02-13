namespace Finbuckle.MultiTenant.Contrib
{
    public interface IValidateTenantRequirement
    {
        bool TenantIsRequired();
    }
}
