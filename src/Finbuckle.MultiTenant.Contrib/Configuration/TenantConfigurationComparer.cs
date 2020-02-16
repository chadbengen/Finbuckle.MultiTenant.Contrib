using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    internal class TenantConfigurationComparer : IEqualityComparer<ITenantConfiguration>
    {
        public bool Equals(ITenantConfiguration x, ITenantConfiguration y)
        {
            return x.Key.Equals(y.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode([DisallowNull] ITenantConfiguration obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
