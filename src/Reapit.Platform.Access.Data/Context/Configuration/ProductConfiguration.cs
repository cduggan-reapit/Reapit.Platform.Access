using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Product"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("products");

        builder.HasIndex(entity => new { entity.Name, entity.DateDeleted })
            .IsUnique();
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
    }
}