using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    public class TenantConfigurations
    {
        public TenantConfigurations() 
            : this(new List<ITenantConfiguration>(), new List<ITenantConfiguration>())
        { }

        public TenantConfigurations(IEnumerable<ITenantConfiguration> manualConfigurations)
         : this(new List<ITenantConfiguration>(), manualConfigurations)
        { }

        public TenantConfigurations(IOptionsSnapshot<TenantAppSettingsConfigurations> appSettingsConfigurations)
            : this(appSettingsConfigurations.Value?.Items, new List<ITenantConfiguration>())
        { }

        public TenantConfigurations(IOptionsSnapshot<TenantAppSettingsConfigurations> appSettingsConfigurations, IEnumerable<ITenantConfiguration> manualConfigurations)
         : this(appSettingsConfigurations.Value?.Items, manualConfigurations)
        { }

        private TenantConfigurations(IEnumerable<ITenantConfiguration> appSettingsConfigurations, IEnumerable<ITenantConfiguration> manualConfigurations)
        {
            var appSettings = appSettingsConfigurations?.ToList() ?? new List<ITenantConfiguration>();
            var manSettings = manualConfigurations ?? new List<ITenantConfiguration>();

            Items = appSettings.Union(manSettings, new TenantConfigurationComparer()).ToList();
        }

        public List<ITenantConfiguration> Items { get; set; }

        public T Get<T>(string key)
        {
            var item = Items.FirstOrDefault(i => i.Key.Equals(key, System.StringComparison.InvariantCultureIgnoreCase));
            var value = item.Value;
            try { 
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
