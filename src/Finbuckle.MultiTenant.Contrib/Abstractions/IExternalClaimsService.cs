using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Abstractions
{
    public interface IExternalClaimsService
    {
        Task<IEnumerable<Claim>> GetClaims(IEnumerable<Claim> currentClaims);
        Task<IEnumerable<Claim>> GetClaims(IEnumerable<Claim> currentClaims, string token);
    }
}
