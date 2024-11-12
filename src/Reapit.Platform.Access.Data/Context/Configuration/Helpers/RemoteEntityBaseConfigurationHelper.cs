using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Data.Context.Configuration.Helpers;

/// <summary>Base configuration methods for entities inheriting <see cref="RemoteEntityBase"/>.</summary>
[ExcludeFromCodeCoverage]
public static class RemoteEntityBaseConfigurationHelper
{
    /// <summary>Configures the entity of type <typeparamref name="T"/>.</summary>
    /// <param name="builder">The entity type builder.</param>
    /// <typeparam name="T">The type of RemoteEntityBase.</typeparam>
    public static EntityTypeBuilder<T> ConfigureRemoteEntityBase<T>(this EntityTypeBuilder<T> builder)
        where T : RemoteEntityBase
    {
        builder.Property(entity => entity.DateLastSynchronised).HasColumnName("last_sync");
        return builder;
    }
}