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

        [Fact]
        public void Get_ConnectionString_Values_Using()
        {
            const string connectionString = "TestDbContext Connection String";
            var t = SharedMock.TestTenantInfo;
            t.SetConnectionString("MyDb", connectionString);
            var cn = t.GetConnectionString("MyDb");

            Assert.Equal(connectionString, cn);
        }
        [Fact]
        public void Get_ConnectionString_Values_Using_T_Name()
        {
            const string connectionString = "TestDbContext Connection String";
            var t = SharedMock.TestTenantInfo;
            t.SetConnectionString<TestDbContext>(connectionString);
            var cn = t.GetConnectionString("TestDbContext");

            Assert.Equal(connectionString, cn);
        }
        [Fact]
        public void Get_ConnectionString_Values_Using_T()
        {
            const string connectionString = "TestDbContext Connection String";
            var t = SharedMock.TestTenantInfo;
            t.SetConnectionString<TestDbContext>(connectionString);
            var cn = t.GetConnectionString<TestDbContext>();

            Assert.Equal(connectionString, cn);
        }
    }
}
