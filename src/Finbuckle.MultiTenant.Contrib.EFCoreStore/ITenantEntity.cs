using System.Collections.Generic;

namespace Finbuckle.MultiTenant.Contrib.EfCoreStore
{
    public interface ITenantEntity
    {
        string ConnectionString { get; set; }
        string Id { get; set; }
        string Identifier { get; set; }
        IDictionary<string, object> Items { get; set; }
        string Name { get; set; }
    }
}