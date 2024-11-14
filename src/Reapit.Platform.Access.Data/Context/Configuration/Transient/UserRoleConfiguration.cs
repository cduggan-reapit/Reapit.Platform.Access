using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

/// <summary>Entity framework configuration for the <see cref="UserRole"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasQueryFilter(entity => entity.Role.DateDeleted == null);

        builder.HasKey(entity => new { entity.UserId, entity.RoleId });

        builder.Property(entity => entity.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36);

        builder.Property(entity => entity.RoleId)
            .HasColumnName("role_id")
            .HasMaxLength(36);

        builder.HasOne(entity => entity.Role)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(entity => entity.RoleId);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.UserRoles)
            .HasForeignKey(entity => entity.UserId);
    }
}