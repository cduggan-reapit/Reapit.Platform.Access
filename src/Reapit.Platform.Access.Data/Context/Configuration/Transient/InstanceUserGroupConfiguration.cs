using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

/// <summary>Entity framework configuration for the <see cref="InstanceUserGroup"/> type.</summary>
[ExcludeFromCodeCoverage]
public class InstanceUserGroupConfiguration : IEntityTypeConfiguration<InstanceUserGroup>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<InstanceUserGroup> builder)
    {
        builder.ToTable("instance_user_groups");

        builder.HasKey(entity => new { entity.InstanceId, entity.UserGroupId });
        
        builder.HasQueryFilter(entity => entity.Instance.DateDeleted == null);

        builder.Property(entity => entity.InstanceId)
            .HasColumnName("instance_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.UserGroupId)
            .HasColumnName("user_group_id")
            .HasMaxLength(36);

        builder.HasOne(entity => entity.Instance)
            .WithMany(instance => instance.InstanceUserGroups)
            .HasForeignKey(entity => entity.InstanceId);

        builder.HasOne(entity => entity.UserGroup)
            .WithMany(userGroup => userGroup.InstanceUserGroups)
            .HasForeignKey(entity => entity.UserGroupId);
    }
}