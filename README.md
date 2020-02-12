# Finbuckle.MultiTenant.Contrib
Contribution functionality for [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant).

## ClaimsStrategy
The ClaimsStrategy adds a strategy for resolving the tenant using the user claims.  In
order for this functionality to work, however, the Finbuckle middleware must be added
AFTER the authentiction middleware.

To use this strategy register it with the WithStrategy and supply an optional string
representing the name of the claim for the tenant id.  If the string is not supplied
then it will default to "TenantId".

```cs
services.AddMultiTenant()
    .WithStrategy<ClaimsStrategy>(ServiceLifetime.Scoped, "TenantId")
    .WithInMemoryStore();
```

## FormStrategy
The FormStrategy adds a strategy for resolving the tenant using form values provided 
by the client.  This is useful if the desired strategy is to use a login screen
where the user provides a tenant id or tenant identifier along with their username
and password.

### FormStrategyConfiguration
Application settings need to be defined for how and when the FormStrategy resolves
the tenant.  Add a section in an appsettings file and define the following:
- **Controller**: The controller that is called.
- **Action**: The action on said controller that is called.
- **Name**: The name of the form value that holds the tenant id or tenant identifier.
- **Type**: An enum indicating if the value is a tenant id or if it is a tenant identifier.

### Registering the FormStrategy
You can either register the FormStrategyConfiguration in the services collection, or
you can supply it as a parameter value.

#### Registering with services
```cs
services.Configure<FormStrategyConfiguration>(ctx.Configuration.GetSection("FormStrategyConfiguration"));

services.AddMultiTenant()
    .WithStrategy<FormStrategy>(ServiceLifetime.Scoped)
    .WithInMemoryStore();
``` 

#### Supplying as parameter value
```cs
services.AddMultiTenant()
    .WithStrategy<FormStrategy>(ServiceLifetime.Scoped, _formStrategyConfiguration)
    .WithInMemoryStore();
``` 

# Finbuckle.MultiTenant.Contrib.EfCoreStore

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