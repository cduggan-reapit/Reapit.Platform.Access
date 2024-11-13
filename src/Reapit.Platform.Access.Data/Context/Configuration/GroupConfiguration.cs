using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Group"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UserGroupConfiguration : IEntityTypeConfiguration<Group>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("groups");
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);
        
        builder.Property(entity => entity.OrganisationId)
            .HasColumnName("organisation_id")
            .HasMaxLength(100);
        
        builder.HasOne(instance => instance.Organisation)
            .WithMany(organisation => organisation.Groups)
            .HasForeignKey(instance => instance.OrganisationId);
    }
}