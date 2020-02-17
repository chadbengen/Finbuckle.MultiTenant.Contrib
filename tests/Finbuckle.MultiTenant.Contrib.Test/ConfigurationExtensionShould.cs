using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    public class ConfigurationExtensionShould
    {
        [Fact]
        public void Return_True()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigurationSectionExTrue).Build();
            var section = configuration.GetSection("TenantConfiguration");

            Assert.True(section.IsMultiTenantEnabled());
        }
        [Fact]
        public void Return_False()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigurationSectionExFalse).Build();
            var section = configuration.GetSection("TenantConfiguration");

            Assert.False(section.IsMultiTenantEnabled());
        }
    }
}
