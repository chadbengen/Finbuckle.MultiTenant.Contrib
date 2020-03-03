using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity
{
    [Obsolete]
    public interface IGenerateClaimFromUser<TUser> where TUser : class
    {
        Task AddClaim(ClaimsIdentity claimsIdentity, TUser user, UserManager<TUser> userManager);
    }

}
