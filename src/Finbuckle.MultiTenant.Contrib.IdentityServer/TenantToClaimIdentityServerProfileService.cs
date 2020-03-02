using System;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer
{
    public class TenantToClaimIdentityServerProfileService<TUser> : ProfileService<TUser>
        where TUser : class
    {
        private readonly string _tenantClaimName;

        public TenantToClaimIdentityServerProfileService(UserManager<TUser> userManager, 
            IUserClaimsPrincipalFactory<TUser> claimsFactory, 
            ILogger<TenantToClaimIdentityServerProfileService<TUser>> logger,
            TenantConfigurations tenantConfigurations) : base(userManager, claimsFactory, logger)
        {
            _tenantClaimName = tenantConfigurations.TenantClaimName();
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);

            Claim tenantClaim = null;

            if (user is IHaveTenantId)
            {
                tenantClaim = new Claim(_tenantClaimName, ((IHaveTenantId)user).TenantId);
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

                if (tenantClaim != null && principal.FindFirst(_tenantClaimName) == null)
                {
                    claims.Add(tenantClaim);
                }

                context.AddRequestedClaims(claims);
            }
        }
    }
}
