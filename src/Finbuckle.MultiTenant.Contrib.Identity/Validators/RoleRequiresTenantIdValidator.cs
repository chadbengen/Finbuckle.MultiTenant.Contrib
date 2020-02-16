using Finbuckle.MultiTenant.Contrib.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity.Validators
{
    /// <summary>
    /// Verifies the role has a tenant id.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    public class RoleRequiresTenantIdValidator<TRole> : TenantIdValidatorBase, IRoleValidator<TRole> where TRole : class
    {
        public RoleRequiresTenantIdValidator(ITenantContext tenantContext) : base(tenantContext)
        { }

        public Task<IdentityResult> ValidateAsync(RoleManager<TRole> manager, TRole role)
        {
            if (role is IHaveTenantId)
            {
                return Validate((IHaveTenantId)role);
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
