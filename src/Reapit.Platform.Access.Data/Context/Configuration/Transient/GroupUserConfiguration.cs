using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

/// <summary>Entity framework configuration for the <see cref="GroupUser"/> type.</summary>
[ExcludeFromCodeCoverage]
public class GroupUserConfiguration : IEntityTypeConfiguration<GroupUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<GroupUser> builder)
    {
        builder.ToTable("user_group_users");

        builder.HasKey(entity => new { UserGroupId = entity.GroupId, entity.OrganisationUserId });
        
        builder.HasQueryFilter(entity => entity.Group.DateDeleted == null);

        builder.Property(entity => entity.GroupId)
            .HasColumnName("user_group_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.OrganisationUserId)
            .HasColumnName("organisation_user_id");

        builder.HasOne(entity => entity.Group)
            .WithMany(userGroup => userGroup.GroupUsers)
            .HasForeignKey(entity => entity.GroupId);
        
        builder.HasOne(entity => entity.OrganisationUser)
            .WithMany(organisationUser => organisationUser.GroupUsers)
            .HasForeignKey(entity => entity.OrganisationUserId);
    }
}