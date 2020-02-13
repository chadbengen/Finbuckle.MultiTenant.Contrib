﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Finbuckle.MultiTenant.Contrib.Identity.Test.Shared
{
    /// <summary>
    ///     EntityType that represents a user belonging to a role
    /// </summary>
    public class PocoUserRole : PocoUserRole<string> { }

    /// <summary>
    ///     EntityType that represents a user belonging to a role
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class PocoUserRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     UserId for the user that is in the role
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public virtual TKey RoleId { get; set; }
    }
}
