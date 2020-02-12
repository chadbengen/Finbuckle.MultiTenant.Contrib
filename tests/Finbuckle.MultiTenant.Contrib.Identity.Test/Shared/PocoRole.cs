using System;

namespace Finbuckle.MultiTenant.Contrib.Identity.Test.Shared
{
    /// <summary>
    ///     Represents a Role entity
    /// </summary>
    public class PocoRole : PocoRole<string>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public PocoRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="roleName"></param>
        public PocoRole(string roleName) : this()
        {
            Name = roleName;
        }
    }
}
