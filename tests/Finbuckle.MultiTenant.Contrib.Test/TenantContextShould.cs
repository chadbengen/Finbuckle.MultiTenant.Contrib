using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Strategies;
using Finbuckle.MultiTenant.Contrib.Test.Common;
using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    public class TenantContextShould
    {
        [Fact]
        public void Resolve_With_TryAddTenantContext()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();
            services.AddHttpContextAccessor();
            services.TryAddTenantContext();
            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            var context = services.BuildServiceProvider().GetService<ITenantContext>();
            var items = context.TenantConfigurations.Items;

            Assert.NotNull(items);
            Assert.Equal(8, items.Count);
            Assert.Equal("TenantId", items.First(a => a.Key == Constants.TenantClaimName).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.MultiTenantEnabled).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.UseTenantCode).Value);
        }

        [Theory]
        [InlineData("TenantId", "initech-id", true)]
        [InlineData("TenantId", "initech-id", false)]
        public async Task Resolve_Tenant(string tenantClaimName, string tenantClaimValue, bool injectConfiguration)
        {
            IWebHostBuilder hostBuilder = GetTestHostBuilder(tenantClaimName, tenantClaimValue, injectConfiguration);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();
                var response = await client.GetStringAsync("/controller/action");
                Assert.Equal("Ok", response);
            }
        }

        private static IWebHostBuilder GetTestHostBuilder(string tenantClaimName, string tenantClaimValue, bool injectConfiguration)
        {
            return new WebHostBuilder()
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     if (injectConfiguration)
                     {
                         configApp.AddInMemoryCollection(SharedMock.ConfigDic);
                     }
                 })
                .ConfigureServices((ctx, services) =>
                {
                    var logger = new Mock<ILogger<ClaimsStrategy>>();
                    services.AddScoped(sp => logger.Object);

                    if (injectConfiguration)
                    {
                        services.AddMultiTenant()
                            .WithContribTenantContext(ctx.Configuration.GetSection("TenantConfiguration"))
                            .WithClaimsStrategy()
                            .WithInMemoryStore();
                    }
                    else
                    {
                        services.AddMultiTenant()
                            .WithContribTenantContext()
                            .WithClaimsStrategy("TenantId")
                            .WithInMemoryStore();
                    }

                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<AuthenticatedTestRequestMiddleware>(SharedMock.GetClaimsIdentity(tenantClaimName, tenantClaimValue));
                    app.UseMultiTenant();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.Map("{controller}/{action}/{id?}", async context =>
                        {
                            var tenantContext = context.RequestServices.GetService<ITenantContext>();
                            Assert.NotNull(tenantContext);
                            Assert.NotNull(tenantContext.TenantConfigurations.Items);
                            Assert.True(tenantContext.TenantResolved);
                            await context.Response.WriteAsync("Ok");
                        });
                    });

                    var store = app.ApplicationServices.GetRequiredService<IMultiTenantStore>();
                    SharedMock.PopulateTestStore(store);
                });
        }

        [Fact]
        public void Not_Resolve_With_ClaimsStrategy()
        {
            var services = new ServiceCollection();

            services.AddMultiTenant().WithClaimsStrategy("TenantId").WithInMemoryStore();

            var tc = services.BuildServiceProvider().GetService<ITenantContext>();

            Assert.Null(tc);
        }

        [Fact]
        public void Not_Resolve_With_FormStrategy()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();
            var services = new ServiceCollection();

            services.AddMultiTenant().WithFormStrategy(configuration.GetSection("TenantConfiguration:FormStrategyConfiguration")).WithInMemoryStore();

            var tc = services.BuildServiceProvider().GetService<ITenantContext>();

            Assert.Null(tc);
        }
    }
}
