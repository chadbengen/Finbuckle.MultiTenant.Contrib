using Finbuckle.MultiTenant.Contrib.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer
{
    /// <summary>
    /// Allows the tenant to be unresolved for Identity Server endpoint requests.
    /// </summary>
    public class TenantNotRequiredForIdentityServerEndpoints : IValidateTenantRequirement
    {
        private readonly IEnumerable<IdentityServer4.Hosting.Endpoint> _endpoints;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantNotRequiredForIdentityServerEndpoints(IEnumerable<IdentityServer4.Hosting.Endpoint> endpoints, IHttpContextAccessor httpContextAccessor)
        {
            _endpoints = endpoints;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TenantIsRequired()
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            var path = httpContext?.Request.Path.Value;

            if (path != null)
            {
                foreach (var endpoint in _endpoints)
                {
                    if (path.ToUpper().EndsWith(endpoint.Path.Value.ToUpper()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
