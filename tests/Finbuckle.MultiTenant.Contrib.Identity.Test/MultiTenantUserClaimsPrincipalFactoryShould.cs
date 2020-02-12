using Finbuckle.MultiTenant.Contrib.Identity.Test.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Identity.Test
{
    public class MultiTenantUserClaimsPrincipalFactoryShould
    {
        /// <summary>
        /// AspNetIdentity test duplicate
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateIdentityNullChecks()
        {
            var userManager = MockHelpers.MockUserManager<PocoUser>().Object;
            var roleManager = MockHelpers.MockRoleManager<PocoRole>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            Assert.Throws<ArgumentNullException>("optionsAccessor",
                () => new MultiTenantUserClaimsPrincipalFactory<PocoUser, PocoRole>(userManager, roleManager, options.Object, MockHelpers.TestTenantContext()));
            var identityOptions = new IdentityOptions();
            options.Setup(a => a.Value).Returns(identityOptions);
            var factory = new MultiTenantUserClaimsPrincipalFactory<PocoUser, PocoRole>(userManager, roleManager, options.Object, MockHelpers.TestTenantContext());
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await factory.CreateAsync(null));
        }

        /// <summary>
        /// Duplicate test from the AspNetIdentity code base substituting the factory class and adding tenantid.
        /// Verifies that the factory throws an error if the user does not have tenant id
        /// Verifies that the factory generates a claim equal to the user tenant id.
        /// </summary>
        /// <param name="supportRoles"></param>
        /// <param name="supportClaims"></param>
        /// <param name="supportRoleClaims"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, true)]
        public async Task EnsureClaimsIdentityHasExpectedClaims(bool supportRoles, bool supportClaims, bool supportRoleClaims)
        {
            // Setup
            var userManager = MockHelpers.MockUserManager<PocoUser>();
            var roleManager = MockHelpers.MockRoleManager<PocoRole>();
            var user = new PocoUser { UserName = "Foo" };
            userManager.Setup(m => m.SupportsUserClaim).Returns(supportClaims);
            userManager.Setup(m => m.SupportsUserRole).Returns(supportRoles);
            userManager.Setup(m => m.GetUserIdAsync(user)).ReturnsAsync(user.Id);
            userManager.Setup(m => m.GetUserNameAsync(user)).ReturnsAsync(user.UserName);
            var roleClaims = new[] { "Admin", "Local" };
            if (supportRoles)
            {
                userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roleClaims);
                roleManager.Setup(m => m.SupportsRoleClaims).Returns(supportRoleClaims);
            }
            var userClaims = new[] { new Claim("Whatever", "Value"), new Claim("Whatever2", "Value2") };
            if (supportClaims)
            {
                userManager.Setup(m => m.GetClaimsAsync(user)).ReturnsAsync(userClaims);
            }
            userManager.Object.Options = new IdentityOptions();

            var admin = new PocoRole() { Name = "Admin" };
            var local = new PocoRole() { Name = "Local" };
            var adminClaims = new[] { new Claim("AdminClaim1", "Value1"), new Claim("AdminClaim2", "Value2") };
            var localClaims = new[] { new Claim("LocalClaim1", "Value1"), new Claim("LocalClaim2", "Value2") };
            if (supportRoleClaims)
            {
                roleManager.Setup(m => m.FindByNameAsync("Admin")).ReturnsAsync(admin);
                roleManager.Setup(m => m.FindByNameAsync("Local")).ReturnsAsync(local);
                roleManager.Setup(m => m.GetClaimsAsync(admin)).ReturnsAsync(adminClaims);
                roleManager.Setup(m => m.GetClaimsAsync(local)).ReturnsAsync(localClaims);
            }

            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            options.Setup(a => a.Value).Returns(identityOptions);
            var roleObject = roleManager.Object;

            var factory = new MultiTenantUserClaimsPrincipalFactory<PocoUser, PocoRole>(userManager.Object, roleManager.Object, options.Object, MockHelpers.TestTenantContext());

            // Act #1
            await Assert.ThrowsAsync<MultiTenantException>(async () => await factory.CreateAsync(user));

            // Arrange #2
            user.TenantId = "initech-id";
            
            // Act
            var principal = await factory.CreateAsync(user);
            var identity = principal.Identities.First();

            // Assert
            var manager = userManager.Object;
            Assert.NotNull(identity);
            Assert.Single(principal.Identities);
            Assert.Equal(IdentityConstants.ApplicationScheme, identity.AuthenticationType);
            var claims = identity.Claims.ToList();
            Assert.NotNull(claims);
            Assert.Contains(
                claims, c => c.Type == manager.Options.ClaimsIdentity.UserNameClaimType && c.Value == user.UserName);
            Assert.Contains(claims, c => c.Type == manager.Options.ClaimsIdentity.UserIdClaimType && c.Value == user.Id);
            Assert.Equal(supportRoles, claims.Any(c => c.Type == manager.Options.ClaimsIdentity.RoleClaimType && c.Value == "Admin"));
            Assert.Equal(supportRoles, claims.Any(c => c.Type == manager.Options.ClaimsIdentity.RoleClaimType && c.Value == "Local"));
            foreach (var cl in userClaims)
            {
                Assert.Equal(supportClaims, claims.Any(c => c.Type == cl.Type && c.Value == cl.Value));
            }
            foreach (var cl in adminClaims)
            {
                Assert.Equal(supportRoleClaims, claims.Any(c => c.Type == cl.Type && c.Value == cl.Value));
            }
            foreach (var cl in localClaims)
            {
                Assert.Equal(supportRoleClaims, claims.Any(c => c.Type == cl.Type && c.Value == cl.Value));
            }
            Assert.Contains(claims, c => c.Type == MockHelpers.TestTenantContext().TenantClaimName);
            Assert.Equal(user.TenantId, claims.First(c => c.Type == MockHelpers.TestTenantContext().TenantClaimName).Value);

            userManager.VerifyAll();
            roleManager.VerifyAll();
        }
    }
}
