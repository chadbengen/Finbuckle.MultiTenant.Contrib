﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Finbuckle.MultiTenant.Contrib.IdentityServer.Test.Shared
{
    /// <summary>
    ///     EntityType that represents one specific user claim
    /// </summary>
    public class PocoUserClaim : PocoUserClaim<string> { }

    /// <summary>
    ///     EntityType that represents one specific user claim
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class PocoUserClaim<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Primary key
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        ///     User Id for the user who owns this claim
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        ///     Claim type
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        ///     Claim value
        /// </summary>
        public virtual string ClaimValue { get; set; }
    }
}
