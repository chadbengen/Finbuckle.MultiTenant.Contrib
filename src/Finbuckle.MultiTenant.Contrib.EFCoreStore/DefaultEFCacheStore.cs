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
using Finbuckle.MultiTenant.Contrib.Configuration;
using Finbuckle.MultiTenant.Contrib.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CareComplete.MultiTenant.Stores
{
    /// <summary>
    /// Default Entity Framework store using the <see cref="DefaultTenantDbContext"/> with a configurable cache.
    /// </summary>
    public class DefaultEFCacheStore : DefaultEFStore
    {
        private readonly DefaultTenantDbContext _dbContext;
        private readonly IDistributedCache _memoryCache;
        private readonly ILogger<DefaultEFCacheStore> _logger;
        private readonly DistributedCacheEntryOptions _cacheOptions = new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(60) };

        private readonly string _cacheKey = $"{typeof(DefaultEFCacheStore).FullName}";

        public DefaultEFCacheStore(DefaultTenantDbContext dbContext, IDistributedCache memoryCache, TenantConfigurations tenantConfigurations, ILogger<DefaultEFCacheStore> logger) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheOptions = new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(tenantConfigurations.CacheMinutes()) };
        }

        private IQueryable<TenantInfo> GetTenantsFromDbContext =>
            _dbContext.Set<TenantEntity>()
                .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, ti.Items));

        private void SetCache(List<TenantInfo> tenants) {
            _logger.LogDebug("Caching {TenantCount} tenants for {CacheMinutes} minutes.", tenants.Count, _cacheOptions.SlidingExpiration.Value.TotalMinutes);
            _memoryCache.Set(_cacheKey, tenants, _cacheOptions); 
        }

        private async Task<List<TenantInfo>> GetTenants()
        {
            var cached = await _memoryCache.GetAsync<List<TenantInfo>>(_cacheKey);

            if (cached == null  )
            {
                cached = await GetTenantsFromDbContext.ToListAsync();

                SetCache(cached);
            }
            else
            {
                await _memoryCache.RefreshAsync(_cacheKey);
            }

            return cached;
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