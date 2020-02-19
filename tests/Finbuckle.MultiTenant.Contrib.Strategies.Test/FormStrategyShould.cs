using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Finbuckle.MultiTenant.Contrib.Strategies;
using Finbuckle.MultiTenant.Contrib.Strategies.Test.Mock;

namespace Finbuckle.MultiTenant.Contrib.Strategies.Test
{
    public class FormStrategyShould
    {
        public static List<KeyValuePair<string, string>> ToFormPostData(Dictionary<string, string> formPostBodyData)
        {
            var result = new List<KeyValuePair<string, string>>();

            formPostBodyData.Keys.ToList().ForEach(key =>
            {
                result.Add(new KeyValuePair<string, string>(key, formPostBodyData[key]));
            });

            return result;
        }

        [Theory]
        [InlineData("/account/login", "initech", true, "initech-id")]
        [InlineData("/account/register", "initech", true, "initech-id")]
        [InlineData("/account/login", "lol", true, "lol-id")]
        [InlineData("/account/register", "lol", true, "lol-id")]
        [InlineData("/account/login", "code-not-exist", true, null)]
        [InlineData("/account/register", "code-not-exist", true, null)]
        [InlineData("/account/somethingelse", "initech", true, null)]
        [InlineData("/account/login", "initech", false, "initech-id")]
        [InlineData("/account/register", "initech", false, "initech-id")]
        [InlineData("/account/login", "lol", false, "lol-id")]
        [InlineData("/account/register", "lol", false, "lol-id")]
        [InlineData("/account/login", "code-not-exist", false, null)]
        [InlineData("/account/register", "code-not-exist", false, null)]
        [InlineData("/account/somethingelse", "initech", false, null)]
        public async Task ReturnExpectedIdentifierAsync(string route, string tenantCode, bool injectConfig, string expected)
        {
            IWebHostBuilder hostBuilder = GetTestHostBuilder("{controller}/{action}", injectConfig);

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, route)
                {
                    Content = new FormUrlEncodedContent(ToFormPostData(new Dictionary<string, string>() { { "TenantCode", tenantCode } }))
                };

                var response = await client.SendAsync(httpRequestMessage);
                string responseString = await response.Content.ReadAsStringAsync();
                responseString = string.IsNullOrWhiteSpace(responseString) ? null : responseString;
                Assert.Equal(expected, responseString);
            }
        }

        [Fact]
        public void ReturnException()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.NormalConfig).Build();
            var services = new ServiceCollection();

            var ex = Assert.Throws<MultiTenantException>(() =>
                services.AddMultiTenant().WithFormStrategy(configuration.GetSection("TenantConfiguration")).WithInMemoryStore());

            Assert.Equal($"The configuration section does not contain any valid settings for the {nameof(FormStrategyConfiguration)}.", ex.Message);
        }
    
        private static IMultiTenantStore PopulateTestStore(IMultiTenantStore store)
        {
            store.TryAddAsync(new TenantInfo("initech-id", "initech", "Initech", "connstring", null)).Wait();
            store.TryAddAsync(new TenantInfo("lol-id", "lol", "Lol, Inc.", "connstring2", null)).Wait();

            return store;
        }

        private static IWebHostBuilder GetTestHostBuilder(string routePattern, bool injectConfiguration)
        {
            return new WebHostBuilder()
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.AddInMemoryCollection(SharedMock.NormalConfig);
                 })
                .ConfigureServices((ctx, services) =>
                {

                    var logger = new Mock<ILogger<FormStrategy>>();
                    services.AddScoped(sp => logger.Object);

                    if (injectConfiguration)
                    {
                        services.AddMultiTenant()
                            .WithFormStrategy(ctx.Configuration.GetSection("TenantConfiguration:FormStrategyConfiguration"))
                            .WithInMemoryStore();
                    }
                    else
                    {
                        var c2 = new FormStrategyConfiguration();
                        ctx.Configuration.GetSection("TenantConfiguration:FormStrategyConfiguration").Bind(c2);

                        services.AddMultiTenant()
                            .WithFormStrategy(c2)
                            .WithInMemoryStore();
                    }

                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseMultiTenant();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.Map(routePattern, async context =>
                        {
                            if (context.GetMultiTenantContext().TenantInfo != null)
                            {
                                await context.Response.WriteAsync(context.GetMultiTenantContext().TenantInfo.Id);
                            }
                        });
                    });

                    var store = app.ApplicationServices.GetRequiredService<IMultiTenantStore>();
                    PopulateTestStore(store);
                });
        }
    }
}
