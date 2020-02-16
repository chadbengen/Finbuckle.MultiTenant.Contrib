using Finbuckle.MultiTenant.Contrib.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity.Validators
{
    /// <summary>
    /// Verifies the user has a tenant id.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserRequiresTenantIdValidator<TUser> : TenantIdValidatorBase, IUserValidator<TUser>
        where TUser : class
    {
        public UserRequiresTenantIdValidator(ITenantContext tenantContext) : base(tenantContext)
        {
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (user is IHaveTenantId)
            {
                return Validate((IHaveTenantId)user);
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }

}
