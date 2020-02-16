using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Finbuckle.MultiTenant.Contrib.IdentityServer.Test.Mock;
using Finbuckle.MultiTenant.Contrib.IdentityServer.Test.Shared;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static IdentityModel.OidcConstants;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer.Test
{
    public class MultiTenantProfileServiceShould
    {
        [Fact]
        public void Construct()
        {
            var userManager = MockHelpers.TestUserManager<PocoUser>(); // Shared.MockHelpers.MockUserManager<UserHasTenantId>().Object;
            var claimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<PocoUser>>().Object;
            var logger = new Mock<ILogger<MultiTenantProfileService<PocoUser>>>().Object;

            var tenantConfigurations = SharedMock.TestTenantConfigurations;

            var subject = new MultiTenantProfileService<PocoUser>(
                    userManager,
                    claimsPrincipalFactory,
                    logger,
                    tenantConfigurations
                );

            Assert.NotNull(subject);
        }
      
        [Fact]
        public async Task Issue_TenantId_Claim()
        {
            var userManager = MockHelpers.TestUserManager<PocoUser>(new NoopUserStore()); // Shared.MockHelpers.MockUserManager<UserHasTenantId>().Object;
            var logger = new Mock<ILogger<MultiTenantProfileService<PocoUser>>>().Object;
            var tenantConfigurations = SharedMock.TestTenantConfigurations;
            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(JwtClaimTypes.Subject, "0") }, "Testing");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<PocoUser>>();
            mockClaimsPrincipalFactory.Setup(c => c.CreateAsync(It.IsAny<PocoUser>())).Returns(Task.FromResult(claimsPrincipal));
            var claimsPrincipalFactory = mockClaimsPrincipalFactory.Object;

            var context = new ProfileDataRequestContext(
                claimsPrincipal,
                new Mock<Client>().Object,
                "client",
                new string[1] { "TenantId" });

            var subject = new MultiTenantProfileService<PocoUser>(
                    userManager,
                    claimsPrincipalFactory,
                    logger,
                    tenantConfigurations
                );

            await subject.GetProfileDataAsync(context);

            Assert.Single(context.IssuedClaims);
        }
    }
}
