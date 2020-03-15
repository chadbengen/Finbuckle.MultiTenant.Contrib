using Microsoft.AspNetCore.Http;
using System;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public static class Helper
    {
        public static void SetTenantCookie(HttpResponse response, string tenantKey, string tenantId)
        {            
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
                        HttpOnly = false,
                        Expires = DateTimeOffset.Now.AddYears(10)
                    }
                );
            }
        }
    }
}
