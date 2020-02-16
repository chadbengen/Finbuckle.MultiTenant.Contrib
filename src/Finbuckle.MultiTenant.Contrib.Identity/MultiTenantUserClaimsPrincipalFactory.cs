using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity
{

    /// <summary>
    /// Creates claims for a multi tenant user ensuring that the TenantId claim is added to the user.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    public class MultiTenantUserClaimsPrincipalFactory<TUser, TUserRole> : UserClaimsPrincipalFactory<TUser, TUserRole> where TUser : class where TUserRole : class
    {
        private readonly string _tenantClaimName;

        public MultiTenantUserClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TUserRole> roleManager,
            IOptions<IdentityOptions> options,
            TenantConfigurations tenantConfigurations) : base(userManager, roleManager, options)
        {
            _tenantClaimName = tenantConfigurations.TenantClaimName();
        }

        /// <summary>
        /// Generate the claims for a user including TenantId.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var id = await base.GenerateClaimsAsync(user);

            if (user is IHaveTenantId)
            {
                // get the tenantid from the user
                string tenantId = ((IHaveTenantId)user).TenantId;

                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    throw new MultiTenantException("User does not have a Tenant Id set.");
                }
                
                // if the tenantid is not already a user claim then we need to add it
                if (!id.Claims.Any(a => a.Type == _tenantClaimName))
                {
                    id.AddClaim(new Claim(_tenantClaimName, tenantId));
                }
            }

            return id;
        }
    }

}
