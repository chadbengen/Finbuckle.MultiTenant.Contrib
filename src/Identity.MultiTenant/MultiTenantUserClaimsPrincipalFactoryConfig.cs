namespace Finbuckle.MultiTenant.Contrib.Identity
{
    /// <summary>
    /// Configuration object for injection into the <see cref="MultiTenantUserClaimsPrincipalFactory{TUser, TUserRole}"/>
    /// </summary>
    public class MultiTenantUserClaimsPrincipalFactoryConfig : ITenantClaimConfiguration
    {
        public MultiTenantUserClaimsPrincipalFactoryConfig(string tenantClaimName)
        {
            TenantClaimName = tenantClaimName;
        }

        public string TenantClaimName { get;}
    }

}
