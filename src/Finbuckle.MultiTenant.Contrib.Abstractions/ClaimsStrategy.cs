﻿using Finbuckle.MultiTenant.Contrib.Configuration;
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

        /// <summary>
        /// Supports the custom WithStrategy implementation.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tenantClaimName"></param>
        public ClaimsStrategy(ILogger<ClaimsStrategy> logger, string tenantClaimName)
        {
            _logger = logger;
            _tenantClaimName = tenantClaimName;

            if (_tenantClaimName == null)
            {
                _tenantClaimName = "TenantId";
                _logger.LogDebug($"TenantClaimName configuration is not set.  Using default: {_tenantClaimName}.");
            }
        }
        public ClaimsStrategy(ILogger<ClaimsStrategy> logger, TenantConfigurations tenantConfigurations)
        {
            _logger = logger;
            _tenantClaimName = tenantConfigurations.TenantClaimName();

            if (_tenantClaimName == null)
            {
                _tenantClaimName = "TenantId";
                _logger.LogDebug($"TenantClaimName configuration is not set.  Using default: {_tenantClaimName}.");
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
}