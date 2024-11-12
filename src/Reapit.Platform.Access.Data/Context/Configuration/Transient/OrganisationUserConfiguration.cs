using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

/// <summary>Entity framework configuration for the <see cref="OrganisationUser"/> type.</summary>
[ExcludeFromCodeCoverage]
public class OrganisationUserConfiguration : IEntityTypeConfiguration<OrganisationUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrganisationUser> builder)
    {
        builder.ConfigureRemoteEntityBase()
            .ToTable("organisation_users");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.OrganisationId)
            .HasColumnName("organisation_id")
            .HasMaxLength(100);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.OrganisationUsers)
            .HasForeignKey(entity => entity.UserId);
        
        builder.HasOne(entity => entity.Organisation)
            .WithMany(organisation => organisation.OrganisationUsers)
            .HasForeignKey(entity => entity.OrganisationId);

    }
}