using Finbuckle.MultiTenant.Contrib.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Finbuckle.MultiTenant.Contrib
{
    /// <summary>
    /// Verifies whether or not tenant resolution is required.  Typically utilized by data access objects which utilize a tenant id for reading or writing.
    /// </summary>
    public class ValidateTenantRequirement
    {
        private readonly IEnumerable<IValidateTenantRequirement> _validators;

        public ValidateTenantRequirement(IEnumerable<IValidateTenantRequirement> validators)
        {
            _validators = validators;
        }

        public bool TenantIsRequired()
        {
            return _validators.All(v => v.TenantIsRequired());
        }
    }
}
