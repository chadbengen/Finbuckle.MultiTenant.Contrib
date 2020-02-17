using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    public class AddTenantConfigurationsShould
    {
        [Fact]
        public void Register()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();
            var items = configurations.Items;

            Assert.NotNull(items);
            Assert.Equal(8, items.Count);
            Assert.Equal("TenantId", items.First(a => a.Key == Constants.TenantClaimName).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.MultiTenantEnabled).Value);
            Assert.Equal("true", items.First(a => a.Key == Constants.UseTenantCode).Value);
        }

        [Fact]
        public void Reload_On_Change()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();

            configuration[$"TenantConfiguration:{Constants.TenantClaimName}"] = "Changed";

            var configurations2 = services.BuildServiceProvider().GetService<TenantConfigurations>();

            Assert.Equal("Changed", configurations2.Items.First(a => a.Key ==Constants.TenantClaimName).Value);
        }

        [Fact]
        public void Combine_With_Manually_Registered_Configurations()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = "Manual1", Value = "Value1" });

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();

            Assert.Equal("Value1", configurations.Items.First(a => a.Key == "Manual1").Value);
        }
 
        [Fact]
        public void Not_Have_Duplicates()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.TenantClaimName, Value = "TenantId-2" });

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();

            Assert.Equal(8, configurations.Items.Count);
            Assert.Equal("TenantId", configurations.Items.First(a => a.Key == Constants.TenantClaimName).Value);
        }

        [Fact]
        public void Use_Last_Registered_When_Registered_2x()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));
            services.AddTenantConfigurations(configuration.GetSection("TenantConfigurationDuplicate"));

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();

            Assert.Single(configurations.Items);
            Assert.Equal("TenantIdDuplicate", configurations.Items.First(a => a.Key == Constants.TenantClaimName).Value);
        }

        [Fact]
        public void Get_Constants()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();

            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            var configurations = services.BuildServiceProvider().GetService<TenantConfigurations>();

            Assert.True(configurations.IsMultiTenantEnabled());
            Assert.True(configurations.UseTenantCode());
            Assert.False(configurations.Get<bool>("bool1"));
            Assert.False(configurations.Get<bool>("bool2"));
            Assert.Equal(0, configurations.Get<int>("int"));
            Assert.Equal(string.Empty, configurations.Get<string>("string"));
        }


    }
}
