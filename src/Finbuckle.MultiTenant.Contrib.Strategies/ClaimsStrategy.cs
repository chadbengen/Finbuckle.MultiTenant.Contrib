using Finbuckle.MultiTenant.Contrib.Claims;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public class ClaimsStrategy : IMultiTenantStrategy
    {
        private readonly ILogger<ClaimsStrategy> _logger;
        private readonly string _tenantClaimName;

        public ClaimsStrategy(ILogger<ClaimsStrategy> logger, TenantConfigurations tenantConfigurations)
        {
            _logger = logger;
            _tenantClaimName = tenantConfigurations.TenantClaimName();

            if (_tenantClaimName == null)
            {
                _tenantClaimName = ContribClaimTypes.TenantId;
            }
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            var tenantId = httpContext?.User?.FindFirst(_tenantClaimName)?.Value;

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();

                var tenantInfo = await store.TryGetAsync(tenantId);

                return tenantInfo?.Identifier;
            }

            return null;
        }
    }
    public static class TenantConfigurationExtensions 
    {
        public static string MultiTenantCookieKey(this TenantConfigurations configurations)
        {
            return configurations.IsMultiTenantEnabled()
                ? configurations.Get<string>(nameof(MultiTenantCookieKey)) 
                    ?? "TenantCookie"
                : null;
        }
    }
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

    //public class SignInStrategy : IMultiTenantStrategy
    //{
    //    private readonly ILogger<SignInStrategy> _logger;

    //    public SignInStrategy(ILogger<SignInStrategy> logger, SignInManager signInManager)
    //    {
    //        _logger = logger;
    //        _tenantClaimName = tenantConfigurations.TenantClaimName();

    //        if (_tenantClaimName == null)
    //        {
    //            _tenantClaimName = ContribClaimTypes.TenantId;
    //        }
    //    }

    //    public async Task<string> GetIdentifierAsync(object context)
    //    {
    //        if (!(context is HttpContext))
    //            throw new MultiTenantException(null,
    //                new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

    //        var httpContext = context as HttpContext;

    //        var result = httpContext.AuthenticateAsync("");


    //        var tenantId = httpContext?.User?.FindFirst(_tenantClaimName)?.Value;

    //        if (!string.IsNullOrWhiteSpace(tenantId))
    //        {
    //            var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();

    //            var tenantInfo = await store.TryGetAsync(tenantId);

    //            return tenantInfo?.Identifier;
    //        }

    //        return null;
    //    }
    //}
}