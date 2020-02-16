using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer.Test.Shared
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class NoopUserStore : IUserStore<PocoUser>
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public Task<string> GetUserIdAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(PocoUser user, string userName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task<IdentityResult> CreateAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<PocoUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = new Mock<PocoUser>().Object;
            user.TenantId = "tenant-id";
            
            return Task.FromResult<PocoUser>(user);
        }

        public Task<PocoUser> FindByNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PocoUser>(null);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
        }

        public Task<IdentityResult> DeleteAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetNormalizedUserNameAsync(PocoUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string>(null);
        }

        public Task SetNormalizedUserNameAsync(PocoUser user, string userName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class NoopRoleStore : IRoleStore<PocoRole>
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public Task<IdentityResult> CreateAsync(PocoRole user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(PocoRole user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetRoleNameAsync(PocoRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task SetRoleNameAsync(PocoRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task<PocoRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<PocoRole>(null);
        }

        public Task<PocoRole> FindByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<PocoRole>(null);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
        }

        public Task<IdentityResult> DeleteAsync(PocoRole user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetRoleIdAsync(PocoRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task<string> GetNormalizedRoleNameAsync(PocoRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }

        public Task SetNormalizedRoleNameAsync(PocoRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}
