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

            _logger.LogDebug("Looking for claim {TenantClaimName}", _tenantClaimName);

            var tenantId = httpContext?.User?.FindFirst(_tenantClaimName)?.Value;

            _logger.LogDebug("Looking for tenant id {tenantId}", tenantId);

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