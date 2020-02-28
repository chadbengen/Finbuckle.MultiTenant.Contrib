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

                var tenantClaim = new Claim(_tenantClaimName, tenantId);
                var existingTenantClaim = id.Claims.FirstOrDefault(a => a.Type == _tenantClaimName);

                // if the tenantid is not already a user claim then we need to add it
                if (existingTenantClaim == null)
                {
                    id.AddClaim(tenantClaim);

                    // save the claim in db
                    await UserManager.AddClaimAsync(user, tenantClaim);
                }
                // ensure that the user tenant id and claim are equal
                else if (!existingTenantClaim.Value.Equals(tenantId, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // replace the claim in db
                    await UserManager.ReplaceClaimAsync(user, existingTenantClaim, tenantClaim);

                    // regenerate identity
                    id = await base.GenerateClaimsAsync(user);
                }
            }

            return id;
        }
    }

}
