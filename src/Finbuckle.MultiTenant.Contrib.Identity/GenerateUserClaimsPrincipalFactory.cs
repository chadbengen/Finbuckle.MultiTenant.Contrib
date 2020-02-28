using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    [Obsolete]
    public interface IGenerateClaimFromUser<TUser> where TUser : class
    {
        Task AddClaim(ClaimsIdentity claimsIdentity, TUser user, UserManager<TUser> userManager);
    }

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

    /// <summary>
    /// Creates claims from <see cref="IGenerateClaimFromUser{TUser}"/> objects.  Overengineered experiment.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    [Obsolete]
    public class GenerateUserClaimsPrincipalFactory<TUser, TUserRole> : UserClaimsPrincipalFactory<TUser, TUserRole> where TUser : class where TUserRole : class
    {
        private readonly IEnumerable<IGenerateClaimFromUser<TUser>> _generators;

        public GenerateUserClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TUserRole> roleManager,
            IOptions<IdentityOptions> options,
            IEnumerable<IGenerateClaimFromUser<TUser>> generators) : base(userManager, roleManager, options)
        {
            _generators = generators;
        }

        /// <summary>
        /// Generate the claims for a user including TenantId.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var id = await base.GenerateClaimsAsync(user);

            foreach (var generator in _generators)
            {
                await generator.AddClaim(id, user, UserManager);
            }

            return id;
        }
    }

}
