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
            .ToTable("groups")
            .Navigation(group => group.Users).AutoInclude();

        // The same name cannot be re-used within an organisation (unless deleted)
        builder.HasIndex(entity => new { entity.OrganisationId, entity.Name, entity.DateDeleted })
            .IsUnique();
        
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
        
        builder.HasMany(group => group.Users)
            .WithMany(user => user.Groups)
            .UsingEntity(joinEntityName: "groupUsers",
                configureLeft: l => l.HasOne(typeof(Group)).WithMany().HasForeignKey("groupId").HasPrincipalKey(nameof(Group.Id)),
                configureRight: r => r.HasOne(typeof(User)).WithMany().HasForeignKey("userId").HasPrincipalKey(nameof(Group.Id)),
                configureJoinEntityType: j => j.HasKey("groupId", "userId"));

    }
}