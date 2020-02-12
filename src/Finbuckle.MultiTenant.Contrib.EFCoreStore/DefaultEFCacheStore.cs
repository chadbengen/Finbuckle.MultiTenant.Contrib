using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Finbuckle.MultiTenant.Contrib.EFCoreStore;
using Microsoft.Extensions.Options;

namespace CareComplete.MultiTenant.Stores
{
    /// <summary>
    /// Default Entity Framework store using the <see cref="DefaultTenantDbContext"/> with a configurable cache.
    /// </summary>
    public class DefaultEFCacheStore : DefaultEFStore
    {
        private readonly DefaultTenantDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(48));
        
        private readonly string _cacheKey = $"{typeof(DefaultEFCacheStore).FullName}";

        public DefaultEFCacheStore(DefaultTenantDbContext dbContext, IMemoryCache memoryCache, IOptionsSnapshot<MultiTenantConfiguration> config) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(config.Value.CacheMinutes));
        }

        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        private IQueryable<TenantInfo> GetTenantsFromDbContext =>                         
            _dbContext.Set<TenantEntity>()
                .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, ti.Items));

        private void SetCache(List<TenantInfo> tenants) => _memoryCache.Set(_cacheKey, tenants, _cacheOptions);

        private async Task<List<TenantInfo>> GetTenants()
        {

            if (!_memoryCache.TryGetValue(_cacheKey, out List<TenantInfo> cachedList))// Look for cache key.
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(_cacheKey, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();

                try
                {
                    if (!_memoryCache.TryGetValue(_cacheKey, out cachedList))
                    {
                        // Key not in cache, so get data.
                        cachedList = await GetTenantsFromDbContext.ToListAsync();

                        SetCache(cachedList);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cachedList;
        }

        public override async Task<TenantInfo> TryGetAsync(string id)
        {
            var cachedTenants = await GetTenants();
            
            var result = cachedTenants.SingleOrDefault(ti => ti.Id == id);

            if (result == null)
            {
                result = await base.TryGetAsync(id);

                if (result != null)
                {
                    _memoryCache.Remove(_cacheKey);
                }
            }

            return result;
        }

        public override async Task<TenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            var cachedTenants = await GetTenants();

            var result = cachedTenants.SingleOrDefault(ti => ti.Identifier == identifier);

            if (result == null)
            {
                var results = await base.TryGetByIdentifierAsync(identifier);

                if (result != null)
                {
                    _memoryCache.Remove(_cacheKey);
                }
            }

            return result;
        }

        public override async Task<bool> TryRemoveAsync(string identifier)
        {
            var cachedTenants = await GetTenants();

            cachedTenants.RemoveAll(t => t.Identifier == identifier);

            SetCache(cachedTenants);

            return await base.TryRemoveAsync(identifier);
        }

        public override async Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
        {
            var cachedTenants = await GetTenants();

            cachedTenants.RemoveAll(t => t.Id == tenantInfo.Id);

            SetCache(cachedTenants);

            return await base.TryUpdateAsync(tenantInfo);
        }
    }
}