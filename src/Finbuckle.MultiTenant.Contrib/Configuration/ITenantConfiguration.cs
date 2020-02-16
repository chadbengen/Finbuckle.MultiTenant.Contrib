using System.Text;

namespace Finbuckle.MultiTenant.Contrib.Configuration
{
    public interface ITenantConfiguration
    {
        string Key { get; set; }
        object Value { get; set; }
    }
}
