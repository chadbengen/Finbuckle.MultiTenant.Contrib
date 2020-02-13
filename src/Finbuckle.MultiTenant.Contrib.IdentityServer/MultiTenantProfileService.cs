using System;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer
{
    public class MultiTenantProfileService<TUser> : ProfileService<TUser>
        where TUser : class
    {
        private readonly ITenantContext _tenantContext;

        public MultiTenantProfileService(UserManager<TUser> userManager, 
            IUserClaimsPrincipalFactory<TUser> claimsFactory, 
            ILogger<ProfileService<TUser>> logger,
            ITenantContext tenantContext) : base(userManager, claimsFactory, logger)
        {
            _tenantContext = tenantContext;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);

            Claim tenantClaim = null;

            if (user is IHaveTenantId)
            {
                tenantClaim = new Claim(_tenantContext.TenantClaimName, ((IHaveTenantId)user).TenantId);
            }
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }
            else
            {
                var principal = await ClaimsFactory.CreateAsync(user);

                if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

                var claims = principal.Claims.ToList();

                if (tenantClaim != null)
                {
                    claims.Add(tenantClaim);
                }

                context.AddRequestedClaims(claims);
            }
        }
    }
}
