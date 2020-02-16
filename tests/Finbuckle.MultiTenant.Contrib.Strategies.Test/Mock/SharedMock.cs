using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib.Strategies.Test.Mock
{
    public static class SharedMock
    {
        public static IConfigurationBuilder GetConfigurationBuilder(Dictionary<string, string> config)
        {

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config);

            return configuration;
        }
        public static Dictionary<string, string> Config =>
            new Dictionary<string, string>()
            {
                {"TenantConfiguration:MultiTenantEnabled", "true" },
                {"TenantConfiguration:UseTenantCode", "false" },
                {"TenantConfiguration:bool1", "" },
                {"TenantConfiguration:bool2", "1" },
                {"TenantConfiguration:int", "" },
                {"TenantConfiguration:string", "" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Controller", "Account" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Action", "Login" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Name", "TenantCode" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:0:Type", "1" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Controller", "Account" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Action", "Register" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Name", "TenantCode" },
                {"TenantConfiguration:FormStrategyConfiguration:Parameters:1:Type", "1" },
            };

        public static Dictionary<string, string> NormalConfig =>
          new Dictionary<string, string>()
          {
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
    }
}
