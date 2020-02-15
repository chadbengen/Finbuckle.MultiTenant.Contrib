# Finbuckle.MultiTenant.Contrib.EfCoreStore
Contribution functionality for [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant).

### DefaultTenantDbContext
The DefaultTenantDbContext implements an Entity Framework Core dbcontext
that also stores the items dictionary.  Note that this functionality lacks an 
comparer.

### DefaultEFCacheStore
A store that uses the DefaultTenantDbContext and caches the tenants with IMemoryCache.
The results are cached for a configuration defined period.

### DefaultEFStore
A non-cached store that uses the DefaultTenantDbContext.

## Using
Register either store using the extension method WithDefaultEFCacheStore or 
WithDefaultEFStore.  When using the cache store you also need to provide a 
configuration section so that the cache time can be added to the service collection.