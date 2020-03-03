using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    [Obsolete]
    /// <summary>
    /// Creates claims from <see cref="IGenerateClaimFromUser{TUser}"/> objects.  Overengineered experiment.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
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
