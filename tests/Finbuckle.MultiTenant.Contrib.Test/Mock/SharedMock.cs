using Bogus;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Finbuckle.MultiTenant.Contrib.Test.Mock
{

    public static class SharedMock
    {
        public static IConfigurationBuilder GetConfigurationBuilder(Dictionary<string, string> config)
        {

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config);

            return configuration;
        }

        public static Dictionary<string, string> ConfigurationSectionExTrue =>
            new Dictionary<string, string>()
            {
                {$"TenantConfiguration:{Constants.MultiTenantEnabled}", "true" },
            };
        public static Dictionary<string, string> ConfigurationSectionExFalse =>
            new Dictionary<string, string>()
            {
                {$"TenantConfiguration:{Constants.MultiTenantEnabled}", "false" },
            };

        public static Dictionary<string, string> ConfigDic =>
          new Dictionary<string, string>()
          {
                {"TenantConfiguration:bool1", "" },
                {"TenantConfiguration:bool2", "1" },
                {"TenantConfiguration:int", "" },
                {"TenantConfiguration:string", "" },
                {$"TenantConfiguration:{Constants.MultiTenantEnabled}", "true" },
                {$"TenantConfiguration:{Constants.UseTenantCode}", "true" },
                {$"TenantConfiguration:{Constants.TenantClaimName}", "TenantId" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Controller", "Account" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Action", "Login" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Name", "TenantCode" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Type", "1" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Controller", "Account" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Action", "Register" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Name", "TenantCode" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Type", "1" },
                {"FormStrategyConfiguration:Parameters:0:Controller", "Account" },
                {"FormStrategyConfiguration:Parameters:0:Action", "Login" },
                {"FormStrategyConfiguration:Parameters:0:Name", "TenantCode" },
                {"FormStrategyConfiguration:Parameters:0:Type", "1" },
                {"FormStrategyConfiguration:Parameters:1:Controller", "Account" },
                {"FormStrategyConfiguration:Parameters:1:Action", "Register" },
                {"FormStrategyConfiguration:Parameters:1:Name", "TenantCode" },
                {"FormStrategyConfiguration:Parameters:1:Type", "1" },
                {$"TenantConfigurationDuplicate:{Constants.TenantClaimName}", "TenantIdDuplicate" },
          };

        public static TenantConfigurations TestTenantConfigurations =>
            new TenantConfigurations
            (
                new List<ITenantConfiguration>()
                {
                    new TenantConfiguration(){Key = Constants.TenantClaimName, Value = "TenantId" },
                    new TenantConfiguration(){Key = Constants.MultiTenantEnabled, Value = "true" },
                    new TenantConfiguration(){Key = Constants.UseTenantCode, Value = "true" },
                }
            );
        public static HttpContextAccessor CreateHttpContextAccessorMock()
        {
            var mock = new Mock<HttpContextAccessor>();

            return mock.Object;
        }
        public static ClaimsIdentity GetClaimsIdentity(string tenantClaimName, string tenantClaimValue) =>
            string.IsNullOrWhiteSpace(tenantClaimName)
            ? new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim("ClaimNameIsWrong", tenantClaimValue),
                }, "Testing")
            : new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(tenantClaimName, tenantClaimValue),
                }, "Testing");
        public static IMultiTenantStore PopulateTestStore(IMultiTenantStore store)
        {
            store.TryAddAsync(new TenantInfo("initech-id", "initech", "Initech", "connstring", null)).Wait();
            store.TryAddAsync(new TenantInfo("lol-id", "lol", "Lol, Inc.", "connstring2", null)).Wait();

            return store;
        }

        public static TenantInfo TestTenantInfo => new Faker<TenantInfo>()
            .RuleFor(r => r.Id, f => Guid.NewGuid().ToString())
            .RuleFor(r => r.Identifier, f => f.Lorem.Word())
            .RuleFor(r => r.ConnectionString, f => f.Lorem.Sentence())
            .RuleFor(r => r.Name, f => f.Lorem.Word())
            .RuleFor(r => r.Items, f => new Dictionary<string, object>()
            {
                { Constants.IsActive, "True" },
                { Constants.RequiresTwoFactorAuthentication, "False" }
            });
    }
}
