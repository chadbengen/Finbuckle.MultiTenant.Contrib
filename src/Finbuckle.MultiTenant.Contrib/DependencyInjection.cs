using Finbuckle.MultiTenant.Contrib;
using Finbuckle.MultiTenant.Contrib.Abstractions;
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

        public static IServiceCollection TryAddTenantContext(this IServiceCollection services)
        {
            services.TryAddValidateTenantRequirement();
            services.TryAddTransient<TenantContext>();
            return services;
        }
        public static IServiceCollection TryAddValidateTenantRequirement(this IServiceCollection services)
        {
            services.TryAddTransient<ValidateTenantRequirement>();
            return services;
        }

        public static IServiceCollection TryAddTenantConfigurations(this IServiceCollection services)
        {
            services.TryAddTenantContext();
            services.TryAddTransient<TenantConfigurations>();
            return services;
        }
        public static IServiceCollection TryAddTenantConfigurations(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.TryAddTenantContext();
            services.Configure<TenantAppSettingsConfigurations>(c =>
                c.Items =
                    configuration
                        .GetChildren()
                        .Select(x => new TenantConfiguration() { Key = x.Key, Value = x.Value })
                        .ToList());
            services.TryAddTransient<TenantConfigurations>();
            return services;
        }
    }
}