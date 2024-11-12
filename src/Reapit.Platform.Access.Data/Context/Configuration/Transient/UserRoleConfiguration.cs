using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Data.Context.Configuration.Transient;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

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