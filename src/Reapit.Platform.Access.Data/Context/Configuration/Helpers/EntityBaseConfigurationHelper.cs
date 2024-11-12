using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Data.Context.Configuration.Helpers;

/// <summary>Base configuration methods for entities inheriting <see cref="EntityBase"/>.</summary>
[ExcludeFromCodeCoverage]
public static class EntityBaseConfigurationHelper
{
    /// <summary>Configures the entity of type <typeparamref name="T"/>.</summary>
    /// <param name="builder">The entity type builder.</param>
    /// <typeparam name="T">The type of EntityBase.</typeparam>
    public static EntityTypeBuilder<T> ConfigureEntityBase<T>(this EntityTypeBuilder<T> builder)
        where T: EntityBase
    {
        /*
         * Filters
         */
        
        builder.HasQueryFilter(entity => entity.DateDeleted == null);
        
        /*
         * Indexes
         */
        
        builder.HasKey(entity => entity.Id);
        builder.HasIndex(entity => entity.Cursor).IsUnique();
        builder.HasIndex(entity => entity.DateDeleted);
        builder.HasIndex(entity => entity.DateModified);
        builder.HasIndex(entity => entity.DateCreated);
        
        /*
         * Properties 
         */
        
        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasMaxLength(36);

        builder.Property(entity => entity.Cursor)
            .HasColumnName("cursor");
        
        builder.Property(entity => entity.DateCreated)
            .HasColumnName("date_created");
        
        builder.Property(entity => entity.DateModified)
            .HasColumnName("date_modified");
        
        builder.Property(entity => entity.DateDeleted)
            .HasColumnName("deleted");
        
        /*
         * Ignored properties
         */

        builder.Ignore(entity => entity.IsDirty);

        return builder;
    }
}