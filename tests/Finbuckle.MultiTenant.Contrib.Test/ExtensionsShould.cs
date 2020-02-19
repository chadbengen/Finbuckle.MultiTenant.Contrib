using Finbuckle.MultiTenant.Contrib.Extensions;
using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Xunit;

namespace Finbuckle.MultiTenant.Contrib.Test
{
    public class ExtensionsShould
    {
        [Fact]
        public void Get_Values_From_TenantInfoExtensions()
        {
            var t = SharedMock.TestTenantInfo;
            var require2fa = t.GetRequiresTwoFactorAuthentication();
            var isActive = t.GetIsActive();

            Assert.False(require2fa);
            Assert.True(isActive);
        }
        [Fact]
        public void Set_Values_On_TenantInfoExtensions()
        {
            var t = SharedMock.TestTenantInfo;
            t.SetIsActive(false);
            t.SetRequiresTwoFactorAuthentication(true);
            
            var require2fa = t.GetRequiresTwoFactorAuthentication();
            var isActive = t.GetIsActive();

            Assert.True(require2fa);
            Assert.False(isActive);
        }
    }
}
