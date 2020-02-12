using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public class FormStrategy : IMultiTenantStrategy
    {
        private readonly ILogger<FormStrategy> _logger;
        private readonly FormStrategyConfiguration _configuration;

        public FormStrategy(ILogger<FormStrategy> logger, IOptionsSnapshot<FormStrategyConfiguration> config) : this(logger, config?.Value)
        { }

        public FormStrategy(ILogger<FormStrategy> logger, FormStrategyConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = config ?? throw new ArgumentNullException(nameof(config));

            if (!_configuration.Parameters?.Any() ?? false)
            {
                throw new MultiTenantException($"No values were provided for the {nameof(FormStrategyConfiguration)}.");
            }
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            if (string.Equals(httpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase)
                && httpContext.Request.HasFormContentType
                && httpContext.Request.Body.CanRead)
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();
                var routeData = httpContext.GetRouteData();
                var controller = (string)routeData.Values["Controller"];
                var action = (string)routeData.Values["action"];
                var parameters = _configuration.Parameters
                    .Where(a => a.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase))
                    .Where(a => a.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                foreach (var r in parameters)
                {
                    var formOptions = new FormOptions { BufferBody = true };
                    var form = await httpContext.Request.ReadFormAsync(formOptions);
                    string value = form[r.Name];

                    if (r.Type == FormStrategyParameterType.Identifier)
                    {
                        var tenantInfo = await store.TryGetByIdentifierAsync(value);

                        if (tenantInfo != null)
                        {
                            _logger.LogDebug($"Returning tenant id for form value: {value}, controller: {controller}, and action: {action}.");
                            return tenantInfo.Identifier;
                        }
                    }
                    else
                    {
                        _logger.LogDebug($"Returning tenant id for form value: {value}, controller: {controller}, and action: {action}.");
                        return value;
                    }

                    _logger.LogDebug($"Tenant could not be found for form value: {value}, controller: {controller}, and action: {action}.");
                }
            }

            return null;
        }
    }

}