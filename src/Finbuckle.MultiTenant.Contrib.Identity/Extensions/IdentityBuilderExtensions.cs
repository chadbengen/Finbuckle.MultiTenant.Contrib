﻿using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Identity;
using Finbuckle.MultiTenant.Contrib.Identity.Stores;
using Finbuckle.MultiTenant.Contrib.Identity.Validators;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IdentityBuilder AddDefaultMultiTenantIdentityServices
            <TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>
            (this IServiceCollection services)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            var builder = new IdentityBuilder(typeof(TUserIdentity), typeof(TUserIdentityRole), services);
            return builder.AddDefaultMultiTenantIdentityServices<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(true);
            //services.AddScoped(typeof(IUserStore<>).MakeGenericType(typeof(TUserIdentity)), typeof(TUserStore));
            //services.AddScoped(typeof(IRoleStore<>).MakeGenericType(typeof(TUserIdentityRole)), typeof(TRoleStore));

            //return services;
        }
    }

    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddDefaultMultiTenantIdentityServices
            <TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>
            (this IdentityBuilder builder)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            return builder.AddDefaultMultiTenantIdentityServices<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(true);
        }

        public static IdentityBuilder AddDefaultMultiTenantIdentityServices
            <TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>
            (this IdentityBuilder builder, bool isMultiTenantEnabled)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            if (isMultiTenantEnabled)
            {
                builder.AddMultiTenantIdentityStores<TUserStore, TRoleStore>();
                builder.AddMultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>();
                builder.AddUserValidator<TUserIdentity, UserRequiresTenantIdValidator<TUserIdentity>>();
                builder.AddUserValidatorRequires2fa<TUserIdentity, UserRequiresTwoFactorAuthenticationValidator<TUserIdentity>, DefaultRequireTwoFactorAuthenticationFactory>();
                builder.AddRoleValidator<TUserIdentityRole, RoleRequiresTenantIdValidator<TUserIdentityRole>>();
            }
            return builder;
        }

        public static IdentityBuilder AddMultiTenantIdentityStores<TUserStore, TRoleStore>(this IdentityBuilder builder)
          where TRoleStore : class
          where TUserStore : class
        {
            builder.AddRoleStore<TRoleStore>();
            builder.AddUserStore<TUserStore>();
            return builder;
        }

        public static IdentityBuilder AddMultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>(this IdentityBuilder builder)
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.AddClaimsPrincipalFactory<MultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>>();
            return builder;
        }
        public static IdentityBuilder AddUserValidator<TUser, TValidator>(this IdentityBuilder builder)
           where TUser : class
           where TValidator : class, IUserValidator<TUser>
        {
            builder.Services.AddScoped(typeof(IUserValidator<TUser>), typeof(TValidator));
            return builder;
        }
        public static IdentityBuilder AddUserValidatorRequires2fa<TUser, TValidator, TRequires2fa>(this IdentityBuilder builder)
           where TUser : class
           where TValidator : class, IUserValidator<TUser>
           where TRequires2fa : class, IRequireTwoFactorAuthenticationFactory
        {
            builder.AddUserValidator<TUser, TValidator>();
            builder.Services.AddScoped<IRequireTwoFactorAuthenticationFactory, TRequires2fa>();
            return builder;
        }
        public static IdentityBuilder AddRoleValidator<TRole, TValidator>(this IdentityBuilder builder)
           where TRole : class
           where TValidator : class, IRoleValidator<TRole>
        {
            builder.Services.AddScoped(typeof(IRoleValidator<TRole>), typeof(TValidator));
            return builder;
        }
    }
}
