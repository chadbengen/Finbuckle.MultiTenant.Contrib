using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public class CookieStrategy : IMultiTenantStrategy
    {
        string _tenantKey;
        public CookieStrategy(TenantConfigurations tenantConfigurations)
        {
            _tenantKey = tenantConfigurations.MultiTenantCookieKey();
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            try
            {
                var tenantId = httpContext.Request?.Cookies[_tenantKey];
                
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    tenantId = AesOperation.DecryptString(AesOperation.Key, tenantId);
                    
                    var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();

                    var tenantInfo = await store.TryGetAsync(tenantId);

                    return tenantInfo?.Identifier;
                }
            }
            catch (Exception)
            {
                // do nothing
            }

            return null;
        }
    }
}