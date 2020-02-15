using Finbuckle.MultiTenant.Contrib.Configuration;
using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib.Abstractions
{
    public interface ITenantContext
    {
        TenantInfo Tenant { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
        string TenantResolutionStrategy { get; }
        void SetTenantId(IHaveTenantId obj);
        TenantConfigurations TenantConfigurations { get; }
    }
}
