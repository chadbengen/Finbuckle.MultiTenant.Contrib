using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Finbuckle.MultiTenant.Contrib.Test.Common
{
    public class AuthenticatedTestRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ClaimsIdentity _identity;

        public AuthenticatedTestRequestMiddleware(RequestDelegate next, ClaimsIdentity identity)
        {
            _next = next;
            _identity = identity;
        }

        public async Task Invoke(HttpContext context)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(_identity);
            context.User = claimsPrincipal;

            await _next(context);
        }
    }
}
