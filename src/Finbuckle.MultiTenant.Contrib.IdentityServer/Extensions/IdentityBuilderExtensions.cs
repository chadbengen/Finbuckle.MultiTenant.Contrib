using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.IdentityServer;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;

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
        public static IIdentityServerBuilder AddTenantToClaimIdentityServerProfileService<TUser>(this IIdentityServerBuilder builder, string tenantClaimName) where TUser : class
        {
            builder.Services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = tenantClaimName });
            builder.Services.AddTenantConfigurations();
            builder.AddProfileService<TenantToClaimIdentityServerProfileService<TUser>>();
            return builder;
        }
        public static IIdentityServerBuilder AddTenantToClaimIdentityServerProfileService<TUser>(this IIdentityServerBuilder builder, IConfigurationSection configurationSection) where TUser : class
        {
            builder.Services.AddTenantConfigurations(configurationSection);
            builder.AddProfileService<TenantToClaimIdentityServerProfileService<TUser>>();
            return builder;
        }
    }

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register default services for IdentityServer.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services) where TUser : class
        {
            services.AddTenantToClaimIdentityServerProfileService<TUser>();
            services.AddTenantNotRequiredForIdentityServerEndpoints();
            return services;
        }
        /// <summary>
        /// Register default services for IdentityServer.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services, IConfigurationSection configurationSection) where TUser : class
        {
            services.AddTenantToClaimIdentityServerProfileService<TUser>(configurationSection);
            services.AddTenantNotRequiredForIdentityServerEndpoints();
            return services;
        }
        /// <summary>
        /// Register default services for IdentityServer.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="tenantClaimName"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services, string tenantClaimName) where TUser : class
        {
            services.AddTenantToClaimIdentityServerProfileService<TUser>(tenantClaimName);
            services.AddTenantNotRequiredForIdentityServerEndpoints();
            return services;
        }

        /// <summary>
        /// Register IdentityServer profile service converting TenantId to user claim using a configuration registered elsewhere.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services) where TUser : class
        {
            var builder = new IdentityServerBuilder(services);
            builder.AddProfileService<TenantToClaimIdentityServerProfileService<TUser>>();
            return services;
        }
        /// <summary>
        /// Register IdentityServer profile service converting TenantId to user claim using a IConfigurationSection that reloads on changes.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services, IConfigurationSection configurationSection) where TUser : class
        {
            services.AddTenantConfigurations(configurationSection);
            services.AddTenantToClaimIdentityServerProfileService<TUser>();
            return services;
        }

        /// <summary>
        /// Register IdentityServer profile service converting TenantId to user claim using the provide TenantClaimName value.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="tenantClaimName"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantToClaimIdentityServerProfileService<TUser>(this IServiceCollection services, string tenantClaimName) where TUser : class
        {
            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = tenantClaimName });
            services.AddTenantConfigurations();

            services.AddTenantToClaimIdentityServerProfileService<TUser>();
            return services;
        }
       
        /// <summary>
        /// Register service indicating a tenant is not required for IdentityServer endpoints.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantNotRequiredForIdentityServerEndpoints(this IServiceCollection services)
        {
            services.AddValidateTenantRequirement();
            services.AddSingleton<IValidateTenantRequirement, TenantNotRequiredForIdentityServerEndpoints>();
            return services;
        }
    }
}
