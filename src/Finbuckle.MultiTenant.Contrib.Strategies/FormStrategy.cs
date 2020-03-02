using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Strategies
{
    public class FormStrategy : IMultiTenantStrategy
    {
        private readonly ILogger<FormStrategy> _logger;
        private readonly FormStrategyConfiguration _configuration;

        public FormStrategy(ILogger<FormStrategy> logger, IOptionsSnapshot<FormStrategyConfiguration> config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = config?.Value ?? throw new ArgumentNullException(nameof(config));

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
            var routeData = httpContext.GetRouteData();
            var hasController = routeData.Values.TryGetValue("controller", out var c1);
            var hasAction = routeData.Values.TryGetValue("action", out var a1);
            string controller = c1?.ToString();
            string action = a1?.ToString();

            var parameters = _configuration.Parameters
                .Where(a => a.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase))
                .Where(a => a.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (parameters.Any())
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();
                var queryData = GetQueryData(httpContext);
                var formData = await GetFormDataAsync(httpContext);

                var data = queryData.Merge(formData).ConvertToCaseInSensitive();

                foreach (var r in parameters)
                {
                    var hasKey = data.ContainsKey(r.Name);

                    if (!hasKey)
                    {
                        continue;
                    }

                    var value = data[r.Name];

                    if (r.Type == FormStrategyParameterType.Identifier)
                    {
                        _logger.LogDebug($"Returning tenant identifier for form value: {value}, controller: {controller}, and action: {action}.");
                        return value;
                    }
                    else
                    {
                        var tenantInfo = await store.TryGetByIdentifierAsync(value);

                        if (tenantInfo != null)
                        {
                            _logger.LogDebug($"Returning tenant id for form value: {value}, controller: {controller}, and action: {action}.");
                            return tenantInfo.Identifier;
                        }
                    }

                    _logger.LogDebug($"Tenant could not be found for form value: {value}, controller: {controller}, and action: {action}.");
                }
            }

            return null;
        }

        private async Task<Dictionary<string, string>> GetFormDataAsync(HttpContext httpContext)
        {
            if (httpContext.Request.HasFormContentType && httpContext.Request.Body.CanRead)
            {
                var formOptions = new FormOptions { BufferBody = true };
                IFormCollection form = await httpContext.Request.ReadFormAsync(formOptions);
                return form.ToDictionary(k => k.Key, v => v.Value.ToString(), StringComparer.InvariantCultureIgnoreCase);
            }
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        private Dictionary<string, string> GetQueryData(HttpContext httpContext)
        {
            if (httpContext.Request.QueryString.HasValue)
            {
                return httpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString(), StringComparer.InvariantCultureIgnoreCase);
            }
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}