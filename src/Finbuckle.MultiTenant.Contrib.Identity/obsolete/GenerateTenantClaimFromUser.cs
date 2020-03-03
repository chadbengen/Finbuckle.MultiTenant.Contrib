using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    [Obsolete]
    public class GenerateTenantClaimFromUser<TUser> : IGenerateClaimFromUser<TUser> where TUser : class
    {
        private readonly string _tenantClaimName;
        public GenerateTenantClaimFromUser(TenantConfigurations tenantConfigurations)
        {
            _tenantClaimName = tenantConfigurations.TenantClaimName();
        }

        public async Task AddClaim(ClaimsIdentity id, TUser user, UserManager<TUser> userManager)
        {
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
                    await userManager.AddClaimAsync(user, tenantClaim);
                }

                // ensure that the user tenant id and claim are equal
                else if (!existingTenantClaim.Value.Equals(tenantId, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // replace the claim in db
                    await userManager.ReplaceClaimAsync(user, existingTenantClaim, tenantClaim);

                    id.RemoveClaim(existingTenantClaim);

                    id.AddClaim(tenantClaim);
                }
            }
        }
    }

}
