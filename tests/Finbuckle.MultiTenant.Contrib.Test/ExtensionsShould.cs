using Finbuckle.MultiTenant.Contrib.Extensions;
using Finbuckle.MultiTenant.Contrib.Test.Mock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Fact]
        public void Get_Object_Using_T()
        {
            MyObject obj = MyObject.Default();
            var t = SharedMock.TestTenantInfo;
            t.Items.Set(nameof(MyObject), obj);
            var objFromTenant = t.Items.SafeGet<MyObject>(nameof(MyObject));

            Assert.Equal(obj.Name, objFromTenant.Name);
        }
        
        [Fact]
        public void Get_Object_Using_T2()
        {
            MyObject obj = MyObject.Default();

            JObject jobj = JObject.FromObject(obj);

            MyObject obj2 = null;

            if (jobj is JObject)
            {
                obj2 = jobj.ToObject<MyObject>();
            }

            Assert.Equal(obj.Name, obj2.Name);
        }

        internal class KeyValue
        {
            [JsonProperty]
            public string Key { get; set; }
            [JsonProperty]
            public object Value { get; set; }
        }

        internal class MyObject
        {
           [JsonConstructor]
            public MyObject()
            {
            }

            public static MyObject Default()
            {
                return new MyObject()
                {
                    Name = "name",
                    Number = 4,
                    Boolean = false,
                    DateTime = new DateTime(2999, 12, 31)
                };
            }

            [JsonProperty]
            public string Name { get; set; }
            [JsonProperty]
            public int Number { get; set; }
            [JsonProperty]
            public bool Boolean { get; set; }
            [JsonProperty]
            public DateTime? DateTime { get; set; }
        }

    }
}
