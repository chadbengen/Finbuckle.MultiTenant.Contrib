using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib
{
    /// <summary>
    /// Defines the configuration values for the <see cref="Strategies.FormStrategy"/>.
    /// </summary>
    public class FormStrategyConfiguration
    {
        public List<FormStrategyParameter> Parameters { get; set; }
    }
    
    /// <summary>
    /// A configuration parameter used by the <see cref="Strategies.FormStrategy"/> to lookup the current tenant.
    /// </summary>
    public class FormStrategyParameter
    {
        /// <summary>
        /// The controller
        /// </summary>
        public string Controller { get; set; }
        
        /// <summary>
        /// The action method
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines how to find the tenant in the <see cref="IMultiTenantStore"/>.
        /// </summary>
        public FormStrategyParameterType Type { get; set; } = FormStrategyParameterType.Identifier;
    }

    public enum FormStrategyParameterType
    {
        /// <summary>
        /// Finds the tenant by <see cref="TenantInfo.Identifier"/>
        /// </summary>
        Identifier = 1,

        /// <summary>
        /// Finds the tenant by <see cref="TenantInfo.Id"/>
        /// </summary>
        Id = 2
    }
}