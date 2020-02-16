using System;
using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    // TODO: How can this be made internal?
    public class TenantAppSettingsConfigurations
    {
        public IEnumerable<ITenantConfiguration> Items { get; set; }

    }
}
