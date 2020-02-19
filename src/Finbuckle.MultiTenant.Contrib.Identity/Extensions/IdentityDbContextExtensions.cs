using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Finbuckle.MultiTenant.Contrib.Identity.Extensions
{
    public static class IdentityDbContextExtensions
    {
        public static void RemoveIndex(this EntityTypeBuilder builder, string propName)
        {
            var prop = builder.Metadata.FindProperty(propName);
            var index = builder.Metadata.FindIndex(prop);
            builder.Metadata.RemoveIndex(index);
        }

        public static TService TryGetService<TService>([NotNull] this IInfrastructure<IServiceProvider> accessor)
        {
            try
            {
                return accessor.GetService<TService>();
            }
            catch
            {
                return default;
            }
        }
    }
}
