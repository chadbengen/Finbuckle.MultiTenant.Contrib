using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.Identity.Abstractions.Stores
{
    public abstract class MultiTenantRoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim> :
        RoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim>
        where TRole : IdentityRole<TKey>, IHaveTenantId
        where TKey : IEquatable<TKey>
        where TContext : DbContext
        where TUserRole : IdentityUserRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private readonly ITenantContext _tenantContext;
        protected string CurrentTenantId => _tenantContext.Tenant?.Id;

        protected MultiTenantRoleStore(TContext context, ITenantContext tenantContext, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _tenantContext = tenantContext;
        }

        public override IQueryable<TRole> Roles => !_tenantContext.TenantResolved && !_tenantContext.TenantResolutionRequired
            // return the roles not filtered if tenant is not required
            ? base.Roles
            // return roles filtered on tenant if tenant is required
            : base.Roles.Where(r => r.TenantId == CurrentTenantId);

        public override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            if (!_tenantContext.TenantResolved && !_tenantContext.TenantResolutionRequired)
            {
                return base.FindByNameAsync(normalizedName, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.TenantId == CurrentTenantId, cancellationToken);
        }
    }

}
