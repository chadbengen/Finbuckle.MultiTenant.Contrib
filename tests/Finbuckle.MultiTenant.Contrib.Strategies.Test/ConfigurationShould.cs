using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Strategies.Test.Mock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    /// <summary>
    /// Verifies the structure of the in memory configuration returns valid results.
    /// </summary>
    public class ConfigurationShould
    {
        [Theory]
        [InlineData("FormStrategyConfiguration")]
        [InlineData("TenantConfiguration:FormStrategyConfiguration")]
        public void FormStrategyConfiguration_Should_Bind(string section)
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.NormalConfig).Build();

            var c2 = new FormStrategyConfiguration();
            configuration.GetSection(section).Bind(c2);
            Assert.NotNull(c2);
            Assert.Equal(2, c2.Parameters.Count);
        }

        [Theory]
        [InlineData("TenantConfiguration")]
        public void Configuration_Should_Bind(string section)
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.NormalConfig).Build();

            var items =
              configuration.GetSection(section)
                  .GetChildren()
                  .Select(x => new TenantConfiguration() { Key = x.Key, Value = x.Value })
                  .ToList();

            Assert.NotNull(items);
            Assert.Equal(4, items.Count);
            Assert.Equal("TenantId", items.First(a => a.Key == Constants.TenantClaimName).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.MultiTenantEnabled).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.UseTenantCode).Value);
        }
    }
}
