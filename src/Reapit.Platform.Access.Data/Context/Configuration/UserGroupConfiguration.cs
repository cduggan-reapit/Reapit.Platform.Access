using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("user_groups");
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.OrganisationId)
            .HasColumnName("organisation_id")
            .HasMaxLength(100);
        
        builder.HasOne(instance => instance.Organisation)
            .WithMany(organisation => organisation.UserGroups)
            .HasForeignKey(instance => instance.OrganisationId);
    }
}