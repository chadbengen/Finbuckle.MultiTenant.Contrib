using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Finbuckle.MultiTenant.Contrib
{
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ValidateTenantRequirement _validateTenantRequirement;

        public TenantContext(
            IHttpContextAccessor httpContextAccessor,
            TenantConfigurations tenantConfigurations,
            ValidateTenantRequirement validateTenantRequirement = null)
        {
            _httpContextAccessor = httpContextAccessor;
            TenantConfigurations = tenantConfigurations ?? new TenantConfigurations();
            _validateTenantRequirement = validateTenantRequirement;
        }

        public TenantInfo Tenant => _httpContextAccessor?.HttpContext?.GetMultiTenantContext()?.TenantInfo;
        public bool TenantResolved => !string.IsNullOrWhiteSpace(Tenant?.Id);
        public bool TenantResolutionRequired => _validateTenantRequirement?.TenantIsRequired() ?? true;
        public string TenantResolutionStrategy => _httpContextAccessor.HttpContext?.GetMultiTenantContext()?.StrategyInfo?.StrategyType?.Name ?? "Unknown";

        public TenantConfigurations TenantConfigurations { get; }

        public void SetTenantId(IHaveTenantId obj)
        {
            if (TenantResolutionRequired && !TenantResolved)
            {
                throw new MultiTenantException("Tenant is not resolved and is missing.");
            }

            obj.TenantId = Tenant.Id;
        }
    }
}
