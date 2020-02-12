using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.Contrib.EFCoreStore
{
    /// <summary>
    /// Default Entity Framework store using the <see cref="DefaultTenantDbContext"/>.
    /// </summary>
    public class DefaultEFStore : IMultiTenantStore
    {
        private readonly DefaultTenantDbContext _dbContext;

        public DefaultEFStore(DefaultTenantDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async virtual Task<TenantInfo> TryGetAsync(string id)
        {
            return await _dbContext.Set<TenantEntity>()
                            .Where(ti => ti.Id == id)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, null))
                            .SingleOrDefaultAsync();
        }

        public async virtual Task<TenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            return await _dbContext.Set<TenantEntity>()
                            .Where(ti => ti.Identifier == identifier)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, null))
                            .SingleOrDefaultAsync();
        }

        public async virtual Task<bool> TryAddAsync(TenantInfo tenantInfo)
        {
            var newEntity = new TenantEntity
            {
                Id = tenantInfo.Id,
                Identifier = tenantInfo.Identifier,
                Name = tenantInfo.Name,
                ConnectionString = tenantInfo.ConnectionString
            };

            await _dbContext.Set<TenantEntity>().AddAsync(newEntity);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async virtual Task<bool> TryRemoveAsync(string identifier)
        {
            var existing = await _dbContext.Set<TenantEntity>().Where(ti => ti.Identifier == identifier).FirstOrDefaultAsync();
            _dbContext.Set<TenantEntity>().Remove(existing);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async virtual Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
        {
            var existing = await _dbContext.Set<TenantEntity>().FindAsync(tenantInfo.Id);
            existing.Identifier = tenantInfo.Identifier;
            existing.Name = tenantInfo.Name;
            existing.ConnectionString = tenantInfo.ConnectionString;

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }

}