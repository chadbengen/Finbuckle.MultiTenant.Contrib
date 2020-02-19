using System;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    public class DefaultRequireTwoFactorAuthenticationFactory : IRequireTwoFactorAuthenticationFactory
    {
        private readonly bool _ignoreWhileDebugging = false;
        public DefaultRequireTwoFactorAuthenticationFactory(TenantConfigurations tenantConfigurations)
        {
            _ignoreWhileDebugging = tenantConfigurations?.Get<bool>(Constants.Ignore2faWhileDebugging) ?? false;
        }

        public Func<TenantInfo, bool> IsRequired => IsRequiredForDebugging;

        private bool IsRequiredForDebugging(TenantInfo tenant)
        {
            var ignore = _ignoreWhileDebugging;
#if !DEBUG
            ignore = false;
#endif
            return ignore ? false : tenant.GetRequiresTwoFactorAuthentication() ?? true;

        }
    }
}
