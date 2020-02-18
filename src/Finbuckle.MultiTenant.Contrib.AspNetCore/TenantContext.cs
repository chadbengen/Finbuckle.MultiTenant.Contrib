using Finbuckle.MultiTenant.Contrib.Abstractions;
using Finbuckle.MultiTenant.Contrib.Configuration;
using Microsoft.AspNetCore.Http;
using Finbuckle.MultiTenant.Contrib.Extensions;

namespace Finbuckle.MultiTenant.Contrib.AspNetCore
{
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ValidateTenantRequirement _validateTenantRequirement;
        private readonly TenantInfo _tenantInfo;

        public TenantContext(
            IHttpContextAccessor httpContextAccessor,
            TenantConfigurations tenantConfigurations,
            TenantInfo tenantInfo,
            ValidateTenantRequirement validateTenantRequirement = null)
        {
            _httpContextAccessor = httpContextAccessor;
            TenantConfigurations = tenantConfigurations ?? new TenantConfigurations();
            _validateTenantRequirement = validateTenantRequirement;
            _tenantInfo = tenantInfo;
        }

        public TenantInfo Tenant => _httpContextAccessor?.HttpContext?.GetMultiTenantContext()?.TenantInfo ?? _tenantInfo;
        public bool TenantResolved => !string.IsNullOrWhiteSpace(Tenant?.Id);
        public bool TenantResolutionRequired => _validateTenantRequirement?.TenantIsRequired() ?? IsMultiTenantEnabled;
        public string TenantResolutionStrategy => _httpContextAccessor.HttpContext?.GetMultiTenantContext()?.StrategyInfo?.StrategyType?.Name ?? "Unknown";
        public bool IsMultiTenantEnabled => TenantConfigurations?.IsMultiTenantEnabled() ?? false;
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
