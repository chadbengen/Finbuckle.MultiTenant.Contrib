using Finbuckle.MultiTenant.Contrib;
using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.AspNetCore;
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
        public static IServiceCollection AddSingleTenantConfiguration(this IServiceCollection services)
        {
            // add the tenant context
            services.TryAddScoped<ITenantContext, TenantContext>();
            // register a configuration for MultiTenantEnabled = false
            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.MultiTenantEnabled, Value = false });
            // register the configurations
            services.AddScoped<TenantConfigurations>();

            return services;
        }

        public static FinbuckleMultiTenantBuilder WithContribTenantContext(this FinbuckleMultiTenantBuilder builder)
        {
            builder.Services.TryAddTenantContext();
            return builder;
        }
        public static FinbuckleMultiTenantBuilder WithContribTenantContext(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            builder.Services.TryAddTenantContext(configurationSection);
            return builder;
        }
        public static IServiceCollection TryAddTenantContext(this IServiceCollection services, IConfigurationSection configurationSection = null)
        {
            services.TryAddScoped<ITenantContext, TenantContext>();

            // add dependencies
            services.AddValidateTenantRequirement();

            if (configurationSection == null)
            {
                services.AddTenantConfigurations();
            }
            else
            {
                services.AddTenantConfigurations(configurationSection);
            }

            return services;
        }
    }
}