using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Contrib;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
        /// <remarks>
        /// The claims strategy does not rely on TenantContext.  Therefore, it is not necessary to
        /// register the tenant context here.  The only thing it relies on is a tenant claim name and this
        /// is registered via a TenantConfiguration object which is injected into the TenantConfigurations
        /// object.  The TenantConfigurations object will handle duplicates in case app settings are also
        /// registered and have a tenant claim name value.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="tenantClaimName">The name of the claim holding the tenant id.</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithClaimsStrategy(this FinbuckleMultiTenantBuilder builder, string tenantClaimName)
        {
            // add the claim name as a manual configuration
            builder.Services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = tenantClaimName });
            // add the configuration wrapper
            builder.Services.AddTenantConfigurations();
            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped);

            return builder;
        }
        /// <summary>
        /// Adds and configures a ClaimsStrategy to the application where the TenantClaimName is registered via IConfigurationSection.
        /// </summary>
        /// <remarks>
        /// The claims strategy does not rely on TenantContext.  Therefore, it is not necessary to
        /// register the tenant context here.  The only thing it relies on is a tenant claim name and this
        /// is registered via a TenantConfiguration object which is injected into the TenantConfigurations
        /// object.  The TenantConfigurations object will handle duplicates in case app settings are also
        /// registered and have a tenant claim name value.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="configurationSection">The configuration section containing a value for <see cref="Constants.TenantClaimName"/>.</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithClaimsStrategy(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            // add the configuration section and configuration wrapper
            builder.Services.AddTenantConfigurations(configurationSection);
            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped);

            return builder;
        }
 
        /// <summary>
        /// Adds and configures a ClaimsStrategy to the application where the TenantClaimName configuration registered elsewhere.
        /// </summary>
        /// <remarks>
        /// The claims strategy does not rely on TenantContext.  Therefore, it is not necessary to
        /// register the tenant context here.  The only thing it relies on is a tenant claim name and this
        /// is registered via a TenantConfiguration object which is injected into the TenantConfigurations
        /// object.  The TenantConfigurations object will handle duplicates in case app settings are also
        /// registered and have a tenant claim name value.
        /// </remarks>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithClaimsStrategy(this FinbuckleMultiTenantBuilder builder)
        {
            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped);

            return builder;
        }
 
        /// <summary>
        /// Adds and configures a FormStrategy to the application with a singleton configuration.
        /// </summary>
        /// <remarks>
        /// The form strategy does not rely on TenantContext.  Therefore, it is not necessary to
        /// register the tenant context here.  The only thing it relies on is a FormStrategyConfiguration
        /// object.  This object is either added as a parameter or injected via IOptionsSnapshot.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="formStrategyConfiguration">A static form strategy configuration</param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithFormStrategy(this FinbuckleMultiTenantBuilder builder, FormStrategyConfiguration formStrategyConfiguration)
        {
            // validate the configuration section
            ValidateFormStrategyConfiguration(formStrategyConfiguration);

            // wrap the configuration value in a IOptionsSnapshot object and register it
            builder.Services.AddSingleton<IOptionsSnapshot<FormStrategyConfiguration>>(new OptionSnapshotFormStrategyConfiguration(formStrategyConfiguration));

            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<FormStrategy>(ServiceLifetime.Scoped);
            return builder;
        }
        /// <summary>
        /// Adds and configures a FormStrategy to the application with a configuration section configuration which reloads with changes.
        /// </summary>
        /// <remarks>
        /// The form strategy does not rely on TenantContext.  Therefore, it is not necessary to
        /// register the tenant context here.  The only thing it relies on is a FormStrategyConfiguration
        /// object.  This object is either added as a parameter or injected via IOptionsSnapshot.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="formStrategyConfiguration">The configuration section defining the form strategy configuration and resolved with IOptionsSnapshot<></param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder WithFormStrategy(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            // validate the configuration section
            var formStrategyConfiguration = new FormStrategyConfiguration();
            configurationSection.Bind(formStrategyConfiguration);

            ValidateFormStrategyConfiguration(formStrategyConfiguration);
            
            // register configuration so it will reload with changes
            builder.Services.Configure<FormStrategyConfiguration>(configurationSection);

            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<FormStrategy>(ServiceLifetime.Scoped);
            
            return builder;
        }
        public static FinbuckleMultiTenantBuilder WithCookieStrategy(this FinbuckleMultiTenantBuilder builder)
        {
            // register the strategy with the built in finbuckly custom strategy
            builder.WithStrategy<CookieStrategy>(ServiceLifetime.Scoped);

            return builder;
        }

        private static void ValidateFormStrategyConfiguration(FormStrategyConfiguration formStrategyConfiguration)
        {
            if (formStrategyConfiguration == null || formStrategyConfiguration.Parameters == null || formStrategyConfiguration.Parameters.Count == 0)
            {
                throw new MultiTenantException($"The configuration section does not contain any valid settings for the {nameof(FormStrategyConfiguration)}.");
            }
        }
    }
}