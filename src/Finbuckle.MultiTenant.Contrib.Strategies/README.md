# Finbuckle.MultiTenant.Contrib
Contribution functionality for [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant).

## ClaimsStrategy
The ClaimsStrategy adds a strategy for resolving the tenant using the user claims.  In
order for this functionality to work, however, the Finbuckle middleware must be added
AFTER the authentiction middleware.

To use this strategy you must provide either a string value representing the name of the 
claim representing the tenant id, or you need to provide a IConfigurationSection holding
a TenantClaimName value.

```cs
services.AddMultiTenant()
    .WithClaimsStrategy("TenantId")
    .WithInMemoryStore();
```
OR

```cs
services.AddMultiTenant()
    .WithClaimsStrategy(ctx.Configuration.GetSection("TenantConfiguration"))
    .WithInMemoryStore();
```

Alternatively, you could use the WithStrategy builder and pass in a 
string with the tenant claim name.

## FormStrategy
The FormStrategy adds a strategy for resolving the tenant using form values provided 
by the client.  This is useful if the desired strategy is to use a login screen
where the user provides a tenant id or tenant identifier along with their username
and password.

```cs
var c2 = GetFormStrategyConfiguration();

services.AddMultiTenant()
    .WithStrategy<FormStrategy>(ServiceLifetime.Scoped, c2)
    .WithInMemoryStore();
``` 

OR

```cs
services.AddMultiTenant()
    .WithFormStrategy(ctx.Configuration.GetSection("TenantConfiguration:FormStrategyConfiguration"))
    .WithInMemoryStore();
``` 

Alternatively, you could use the WithStrategy builder and pass in the
FormStrategyConfiguration object.

### FormStrategyConfiguration
Application settings need to be defined for how and when the FormStrategy resolves
the tenant.  Add a section in an appsettings file and define the following:
- **Controller**: The controller that is called.
- **Action**: The action on said controller that is called.
- **Name**: The name of the form value that holds the tenant id or tenant identifier.
- **Type**: An enum indicating if the value is a tenant id or if it is a tenant identifier.
