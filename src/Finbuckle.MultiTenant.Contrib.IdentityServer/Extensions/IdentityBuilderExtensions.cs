using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.IdentityServer;
using IdentityServer4.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds the profile service and a object for resolving the tenant claim name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="tenantClaimName"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddMultiTenantProfileService<TUser>(this IIdentityServerBuilder builder, string tenantClaimName) where TUser : class
        {
            builder.Services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = tenantClaimName });
            builder.Services.TryAddTenantConfigurations();
            builder.AddProfileService<MultiTenantProfileService<TUser>>();
            return builder;
        }

        public static IServiceCollection AddTenantNotRequiredForIdentityServerEndpoints(this IServiceCollection services)
        {
            services.TryAddValidateTenantRequirement();
            services.AddSingleton<IValidateTenantRequirement, TenantNotRequiredForIdentityServerEndpoints>();
            return services;
        }
    }
}
