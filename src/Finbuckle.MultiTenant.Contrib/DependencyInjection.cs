using Finbuckle.MultiTenant.Contrib;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provices builder methods for Skoruba.Multitenancy services and configuration.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddValidateTenantRequirement(this IServiceCollection services)
        {
            services.TryAddScoped<ValidateTenantRequirement>();
            return services;
        }

        public static IServiceCollection AddTenantConfigurations(this IServiceCollection services)
        {
            services.TryAddScoped<TenantConfigurations>();
            return services;
        }
        
        public static IServiceCollection AddTenantConfigurations(this IServiceCollection services, IConfigurationSection configuration)
        {
            // add the settings
            services.Configure<TenantAppSettingsConfigurations>(c =>
                c.Items =
                    configuration
                        .GetChildren()
                        .Select(x => new TenantConfiguration() { Key = x.Key, Value = x.Value })
                        .ToList());
            
            // add the service that wraps the configurations
            services.TryAddScoped<TenantConfigurations>();
            return services;
        }
    }
}