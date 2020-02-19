using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using Finbuckle.MultiTenant.Stores;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Finbuckle.MultiTenant.Contrib.Strategies.Test.Mock;
using Finbuckle.MultiTenant.Contrib.Strategies.Test.Common;
using Finbuckle.MultiTenant.Contrib.Abstractions;

namespace Finbuckle.MultiTenant.Contrib.Strategies.Test
{
    public class ClaimStrategyShould
    {
        private HttpContext CreateHttpContextMock(string claimName, string tenantIdClaimValue)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IMultiTenantStore)))
                .Returns(PopulateTestStore(new InMemoryStore(false)));

            var mock = new Mock<HttpContext>();
            if (tenantIdClaimValue != null)
            {
                mock.Setup(c => c.User.FindFirst(It.IsAny<string>())).Returns(new Claim(claimName, tenantIdClaimValue));
            }
            mock.Setup(c => c.RequestServices).Returns(serviceProvider.Object);

            return mock.Object;
        }

        private static IMultiTenantStore PopulateTestStore(IMultiTenantStore store)
        {
            store.TryAddAsync(new TenantInfo("initech-id", "initech", "Initech", "connstring", null)).Wait();
            store.TryAddAsync(new TenantInfo("lol-id", "lol", "Lol, Inc.", "connstring2", null)).Wait();

            return store;
        }

        private static ClaimsIdentity GetClaimsIdentity(string tenantClaimName, string tenantClaimValue) =>
            string.IsNullOrWhiteSpace(tenantClaimName)
            ? new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim("ClaimNameIsWrong", tenantClaimValue),
                }, "Testing")
            : new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(tenantClaimName, tenantClaimValue),
                }, "Testing");


        /// <summary>
        /// Verifies that the ClaimsStrategy returns the correct identifier.
        /// If the user has a tenant claim then the tenant should be returned.
        /// If the user does not have a tenant claim then the tenant result should be null.
        /// If the user has a tenant claim not found then the tenant result should be null.
        /// </summary>
        /// <param name="tenantIdClaimValue"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("TenantId", "initech-id", "initech")]
        [InlineData("TenantId", "lol-id", "lol")]
        [InlineData("TenantId", "initech-id-not-exist", null)]
        [InlineData("TenantId", null, null)]
        [InlineData("Tenant-Id", "initech-id", "initech")]
        [InlineData("Tenant-Id", "lol-id", "lol")]
        [InlineData("Tenant-Id", "initech-id-not-exist", null)]
        [InlineData("Tenant-Id", null, null)]
        public async Task ReturnExpectedIdentifier(string claimName, string tenantIdClaimValue, string expected)
        {
            var logger = new Mock<ILogger<ClaimsStrategy>>();
            var httpContext = CreateHttpContextMock(claimName, tenantIdClaimValue);
            var strategy = new ClaimsStrategy(logger.Object, SharedMock.TestTenantConfigurations);
            var identifier = await strategy.GetIdentifierAsync(httpContext);
            Assert.Equal(expected, identifier);
        }

        /// <summary>
        /// Verifies that the <see cref="ClaimsStrategy"/> is successuful within the Finbuckle middleware.
        /// </summary>
        /// <param name="tenantClaimName"></param>
        /// <param name="tenantClaimValue"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("TenantId", "initech-id", "initech", true)]
        [InlineData("TenantId", "lol-id", "lol", true)]
        [InlineData("TenantId", "initech-id-not-exist", null, true)]
        [InlineData("Tenant-Id", "initech-id", null, true)]
        [InlineData("TenantId", "initech-id", "initech", false)]
        [InlineData("TenantId", "lol-id", "lol", false)]
        [InlineData("TenantId", "initech-id-not-exist", null, false)]
        [InlineData("Tenant-Id", "initech-id", null, false)]
        public async Task ReturnExpectedIdentifierFromHostAsync(string tenantClaimName, string tenantClaimValue, string expected, bool injectConfiguration)
        {
            IWebHostBuilder hostBuilder = GetTestHostBuilder(tenantClaimName, tenantClaimValue, injectConfiguration);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();
                var response = await client.GetStringAsync("/controller/action");
                response = string.IsNullOrWhiteSpace(response) ? null : response;
                Assert.Equal(expected, response);
            }
        }

        private static IWebHostBuilder GetTestHostBuilder(string tenantClaimName, string tenantClaimValue, bool injectConfiguration)
        {
            return new WebHostBuilder()
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     if (injectConfiguration)
                     {
                         configApp.AddInMemoryCollection(SharedMock.NormalConfig);
                     }
                 })
                .ConfigureServices((ctx, services) =>
                {
                    var logger = new Mock<ILogger<ClaimsStrategy>>();
                    services.AddScoped(sp => logger.Object);

                    if (injectConfiguration)
                    {
                        services.AddMultiTenant()
                            .WithClaimsStrategy(ctx.Configuration.GetSection("TenantConfiguration"))
                            .WithInMemoryStore();
                    }
                    else
                    {
                        services.AddMultiTenant()
                            .WithClaimsStrategy("TenantId")
                            .WithInMemoryStore();
                    }

                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<AuthenticatedTestRequestMiddleware>(GetClaimsIdentity(tenantClaimName, tenantClaimValue));
                    app.UseMultiTenant();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.Map("{controller}/{action}/{id?}", async context =>
                        {
                            var tenant = context.GetMultiTenantContext().TenantInfo;

                            if (tenant != null)
                            {
                                await context.Response.WriteAsync(tenant.Identifier);
                            }
                        });
                    });

                    var store = app.ApplicationServices.GetRequiredService<IMultiTenantStore>();
                    PopulateTestStore(store);
                });
        }

    }
}
