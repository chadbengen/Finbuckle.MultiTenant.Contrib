using System;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    public interface IRequireTwoFactorAuthenticationFactory
    {
        Func<TenantInfo, bool> IsRequired { get; }
    }
}
