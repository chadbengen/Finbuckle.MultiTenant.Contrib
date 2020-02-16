using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Contrib;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Strategies;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provices builder methods for Skoruba.Multitenancy services and configuration.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds and configures a ClaimsStrategy to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tenantClaimName">The name of the claim holding the tenant id.</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithClaimsStrategy(this FinbuckleMultiTenantBuilder builder, string tenantClaimName)
        {
            builder.Services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = tenantClaimName });
            builder.Services.TryAddTenantConfigurations();
            builder.WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped);
            return builder;
        }
        /// <summary>
        /// Adds and configures a ClaimsStrategy to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationSection">The configuration section containing a value for <see cref="Constants.TenantClaimName"/>.</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithClaimsStrategy(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            // validate the configuration section
            builder.Services.TryAddTenantConfigurations(configurationSection);
            builder.WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped);
            return builder;
        }
        /// <summary>
        /// Adds and configures a FormStrategy to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="formStrategyConfiguration">A static form strategy configuration</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithFormStrategy(this FinbuckleMultiTenantBuilder builder, FormStrategyConfiguration formStrategyConfiguration)
        {
            // validate the configuration section
            if (formStrategyConfiguration == null || formStrategyConfiguration.Parameters == null || formStrategyConfiguration.Parameters.Count == 0)
            {
                throw new MultiTenantException($"The configuration section does not contain any valid settings for the {nameof(FormStrategyConfiguration)}.");
            }
            builder.Services.TryAddTenantContext();
            builder.Services.TryAddTenantConfigurations();
            builder.WithStrategy<FormStrategy>(ServiceLifetime.Scoped, formStrategyConfiguration);
            return builder;
        }
        /// <summary>
        /// Adds and configures a FormStrategy to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="formStrategyConfiguration">The configuration section defining the form strategy configuration and resolved with IOptionsSnapshot<></param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithFormStrategy(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            // validate the configuration section
            var c = new FormStrategyConfiguration();
            configurationSection.Bind(c);
            if (c == null || c.Parameters == null || c.Parameters.Count == 0)
            {
                throw new MultiTenantException($"The configuration section does not contain any valid settings for the {nameof(FormStrategyConfiguration)}.");
            }
            builder.Services.TryAddTenantConfigurations();
            builder.Services.Configure<FormStrategyConfiguration>(configurationSection);
            builder.WithStrategy<FormStrategy>(ServiceLifetime.Scoped);
            return builder;
        }
    }
}