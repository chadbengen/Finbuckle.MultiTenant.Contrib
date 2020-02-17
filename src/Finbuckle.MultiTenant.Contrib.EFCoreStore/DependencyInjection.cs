//    Copyright 2018 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using Microsoft.EntityFrameworkCore;
using CareComplete.MultiTenant.Stores;
using System;
using Finbuckle.MultiTenant.Contrib.EFCoreStore;
using Microsoft.Extensions.Configuration;
using Finbuckle.MultiTenant.Contrib.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provices builder methods for Skoruba.Multitenancy services and configuration.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds an Entity Framework store using the <see cref="DefaultTenantDbContext"/> and caches tenants for a configurable period of time.
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder WithDefaultEFCacheStore(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddTenantConfigurations(configurationSection);
            builder.Services.AddDbContext<DefaultTenantDbContext>(options); // Note, will not override existing context if already added.
            return builder.WithStore<DefaultEFCacheStore>(ServiceLifetime.Scoped);
        }
        /// <summary>
        /// Adds an Entity Framework store using the <see cref="DefaultTenantDbContext"/> and caches tenants for a configurable period of time.
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder WithDefaultEFCacheStore(this FinbuckleMultiTenantBuilder builder, int cacheMinutes, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddSingleton<ITenantConfiguration>(new TenantConfiguration() { Key = Constants.CacheMinutes, Value = cacheMinutes });
            builder.Services.AddTenantConfigurations();
            builder.Services.AddDbContext<DefaultTenantDbContext>(options); // Note, will not override existing context if already added.
            return builder.WithStore<DefaultEFCacheStore>(ServiceLifetime.Scoped);
        }
        /// <summary>
        /// Adds an Entity Framework store using the <see cref="DefaultTenantDbContext"/>.
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder WithDefaultEFStore(this FinbuckleMultiTenantBuilder builder, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddDbContext<DefaultTenantDbContext>(options); // Note, will not override existing context if already added.
            return builder.WithStore<DefaultEFStore>(ServiceLifetime.Scoped);
        }
    }
}