using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    public class ValidateTenantRequirementShould
    {
        internal class TrueRequirement : IValidateTenantRequirement
        {
            public bool TenantIsRequired()
            {
                return true;
            }
        }
        internal class FalseRequirement : IValidateTenantRequirement
        {
            public bool TenantIsRequired()
            {
                return false;
            }
        }

        [Fact]
        public void Resolve_False()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();
            services.AddHttpContextAccessor();
            services.TryAddTenantContext();
            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));
            services.AddSingleton<IValidateTenantRequirement>(new FalseRequirement());

            var context = services.BuildServiceProvider().GetService<ITenantContext>();
            Assert.False(context.TenantResolutionRequired);
        }
        [Fact]
        public void Resolve_False_With_TrueAndFalse()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();
            services.AddHttpContextAccessor();
            services.TryAddTenantContext();
            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));
            services.AddSingleton<IValidateTenantRequirement>(new FalseRequirement());
            services.AddSingleton<IValidateTenantRequirement>(new TrueRequirement());

            var context = services.BuildServiceProvider().GetService<ITenantContext>();
            Assert.False(context.TenantResolutionRequired);
        }
        [Fact]
        public void Resolve_True_By_Default()
        {
            var configuration = SharedMock.GetConfigurationBuilder(SharedMock.ConfigDic).Build();

            var services = new ServiceCollection();
            services.AddHttpContextAccessor();
            services.TryAddTenantContext();
            services.AddTenantConfigurations(configuration.GetSection("TenantConfiguration"));

            var context = services.BuildServiceProvider().GetService<ITenantContext>();
            Assert.True(context.TenantResolutionRequired);
        }
    }
}
