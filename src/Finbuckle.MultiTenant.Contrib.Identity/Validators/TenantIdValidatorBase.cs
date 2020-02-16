using Finbuckle.MultiTenant.Contrib.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity.Validators
{
    public abstract class TenantIdValidatorBase
    {
        private readonly ITenantContext _tenantContext;

        public TenantIdValidatorBase(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        protected Task<IdentityResult> Validate(IHaveTenantId obj)
        {
            if (_tenantContext.TenantResolutionRequired && !_tenantContext.TenantResolved)
            {
                throw new MultiTenantException("Tenant is not resolved and is missing.");
            }
            if (string.IsNullOrEmpty(obj.TenantId))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Required", Description = "A tenant id is required." }));
            }
            if (obj.TenantId != _tenantContext.Tenant.Id)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Invalid", Description = "The tenant id must be the same as the current user's tenant id." }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }

}
