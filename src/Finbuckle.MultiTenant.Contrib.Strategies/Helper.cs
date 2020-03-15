using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Http;
using System;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public static class Helper
    {
        public static void RemoveTenantCookie(HttpResponse httpResponse, ITenantContext tenantContext)
        {
            var tenantKey = tenantContext.TenantConfigurations.MultiTenantCookieKey();
            httpResponse.Cookies.Delete(tenantKey);
        }

        public static void SetTenantCookie(HttpResponse response, ITenantContext tenantContext)
        {
            var tenantKey = tenantContext.TenantConfigurations.MultiTenantCookieKey();
            var tenantId = tenantContext.Tenant?.Id;
            
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                response.Cookies.Delete(tenantKey);
            }
            else
            {
                var encrypted = AesOperation.EncryptString(AesOperation.Key, tenantId);
                //TODO: encrypt value
                response.Cookies.Append(
                    tenantKey,
                    encrypted,
                    new CookieOptions
                    {
                        Path = "/",
                        HttpOnly = false
                    }
                );
            }
        }
    }
}
