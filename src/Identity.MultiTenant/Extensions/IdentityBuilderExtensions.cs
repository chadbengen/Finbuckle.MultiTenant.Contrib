using Finbuckle.MultiTenant.Contrib.Identity;
using Finbuckle.MultiTenant.Contrib.Identity.Validators;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddDefaultMultiTenantIdentityServices
            <TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>
            (this IdentityBuilder builder, string tenantClaimName)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            return builder.AddDefaultMultiTenantIdentityServices<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(tenantClaimName, true);
        }
        
        public static IdentityBuilder AddDefaultMultiTenantIdentityServices
            <TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>
            (this IdentityBuilder builder, string tenantClaimName, bool isMultiTenantEnabled)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            if (isMultiTenantEnabled)
            {
                builder.AddMultiTenantIdentityStores<TUserStore, TRoleStore>();
                builder.AddMultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>(tenantClaimName);
                builder.AddUserValidator<TUserIdentity, UserRequiresTenantIdValidator<TUserIdentity>>();
                builder.AddUserValidator<TUserIdentity, UserRequiresTwoFactorAuthenticationValidator<TUserIdentity>>();
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

        public static IdentityBuilder AddMultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>(this IdentityBuilder builder, string tenantClaimName)
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
        public static IdentityBuilder AddRoleValidator<TRole, TValidator>(this IdentityBuilder builder)
           where TRole : class
           where TValidator : class, IRoleValidator<TRole>
        {
            builder.Services.AddScoped(typeof(IRoleValidator<TRole>), typeof(TValidator));
            return builder;
        }
    }
}
