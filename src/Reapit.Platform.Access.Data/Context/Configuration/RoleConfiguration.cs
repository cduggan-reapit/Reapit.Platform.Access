using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Role"/> type.</summary>
[ExcludeFromCodeCoverage]
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("roles");
        
        // Role name must be unique
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);
        
        builder.HasMany(role => role.Users)
            .WithMany(user => user.Roles)
            .UsingEntity(joinEntityName: "userRoles",
                configureLeft: l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("roleId").HasPrincipalKey(nameof(Role.Id)),
                configureRight: r => r.HasOne(typeof(User)).WithMany().HasForeignKey("userId").HasPrincipalKey(nameof(User.Id)),
                configureJoinEntityType: j => j.HasKey("roleId", "userId"));
            
    }
}