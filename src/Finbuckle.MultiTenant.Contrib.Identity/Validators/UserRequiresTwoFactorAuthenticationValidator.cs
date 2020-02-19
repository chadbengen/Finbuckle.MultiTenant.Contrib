using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity.Validators
{

    public class UserRequiresTwoFactorAuthenticationValidator<TUser> : IUserValidator<TUser>
        where TUser : class
    {
        private readonly ITenantContext _tenantContext;
        private readonly IRequireTwoFactorAuthenticationFactory _2faRequiredFactory;
        private readonly bool _autoEnable = true;

        public UserRequiresTwoFactorAuthenticationValidator(ITenantContext tenantContext, IRequireTwoFactorAuthenticationFactory factory)
        {
            _tenantContext = tenantContext;
            _2faRequiredFactory = factory;
#if DEBUG
            _autoEnable = tenantContext.TenantConfigurations.Get<bool>(Constants.Ignore2faWhileDebugging);
#endif
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (user is IdentityUser)
            {
                IdentityUser identityUser = user as IdentityUser;

                if (identityUser.TwoFactorEnabled)
                {
                    return Task.FromResult(IdentityResult.Success);
                }

                if (_tenantContext.TenantResolutionRequired && !_tenantContext.TenantResolved)
                {
                    throw new MultiTenantException("Tenant is not resolved and is missing.");
                }

                var isRequired = _2faRequiredFactory.IsRequired(_tenantContext.Tenant);

                if (isRequired && !identityUser.TwoFactorEnabled)
                {
                    if (_autoEnable)
                    {
                        identityUser.TwoFactorEnabled = true;
                        return Task.FromResult(IdentityResult.Success);
                    }

                    return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "2fa_Required", Description = "Two factor authentication is required." }));
                }

            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
