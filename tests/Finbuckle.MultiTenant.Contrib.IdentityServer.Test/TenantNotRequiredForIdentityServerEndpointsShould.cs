using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer.Test
{
    public class TenantNotRequiredForIdentityServerEndpointsShould
    {

        [Theory]
        [InlineData("/register/login", "true")]
        [InlineData("/.well-known/openid-configuration/no", "true")]
        [InlineData("/well-known/openid-configuration/jwks", "true")]
        [InlineData("/.well-known/openid-configuration/jwks", "false")]
        [InlineData("/connect/auth", "true")]
        [InlineData("/connect/authorize", "false")]
        [InlineData("/connect/authorize/callback", "false")]
        [InlineData("/connect/token", "false")]
        [InlineData("/connect/userinfo", "false")]
        [InlineData("/connect/endsession", "false")]
        [InlineData("/connect/endsession/callback", "false")]
        [InlineData("/connect/checksession", "false")]
        [InlineData("/connect/revocation", "false")]
        [InlineData("/connect/introspect", "false")]
        [InlineData("/connect/deviceauthorization", "false")]
        public async Task ReturnExpectedIdentifierFromHostAsync(string endpointToCall, string expected)
        {
            IWebHostBuilder hostBuilder = GetTestHostBuilder();

            using (var server = new TestServer(hostBuilder))
            {
                var client = server.CreateClient();
                var response = await client.GetStringAsync(endpointToCall);
                response = string.IsNullOrWhiteSpace(response) ? null : response;
                Assert.Equal(expected, response);
            }
        }

        private static IWebHostBuilder GetTestHostBuilder()
        {
            return new WebHostBuilder()
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                 })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddTenantNotRequiredForIdentityServerEndpoints();
                    services.AddIdentityServer();
                    services.AddHttpContextAccessor();
                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.Map("{controller}/{action}/{id?}", async context =>
                        {
                            var req = context.RequestServices.GetRequiredService<ValidateTenantRequirement>();
                            await context.Response.WriteAsync(req.TenantIsRequired().ToString().ToLower());
                        });
                    });
                });
        }

    }
}
