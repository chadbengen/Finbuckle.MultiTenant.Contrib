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
        private readonly string _tenantIdClaim;

        public ClaimsStrategy(ILogger<ClaimsStrategy> logger, string tenantIdClaim)
        {
            _logger = logger;
            _tenantIdClaim = tenantIdClaim ?? "TenantId";

            if (tenantIdClaim == null)
            {
                _logger.LogDebug($"TenantIdClaim was not provided.  Using default of {_tenantIdClaim}.");
            }
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            var tenantId = httpContext?.User?.FindFirst(_tenantIdClaim)?.Value;

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();

                var tenantInfo = await store.TryGetAsync(tenantId);

                return tenantInfo?.Identifier;
            }

            return null;
        }
    }
}