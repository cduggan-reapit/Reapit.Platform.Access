using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Instance"/> type.</summary>
[ExcludeFromCodeCoverage]
public class InstanceConfiguration : IEntityTypeConfiguration<Instance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Instance> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("instances");

        // Instance name must be unique by product and organisation
        // E.g. RPT could have two "RES" instances provided one was for AgencyCloud and the other was for Console.
        builder.HasIndex(entity => new { entity.Name, entity.ProductId, entity.OrganisationId, entity.DateDeleted })
            .IsUnique();
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.Property(entity => entity.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(36);
        
        builder.Property(entity => entity.OrganisationId)
            .HasColumnName("organisation_id")
            .HasMaxLength(36);

        builder.HasOne(instance => instance.Product)
            .WithMany(product => product.Instances)
            .HasForeignKey(instance => instance.ProductId);
        
        builder.HasOne(instance => instance.Organisation)
            .WithMany(organisation => organisation.Instances)
            .HasForeignKey(instance => instance.OrganisationId);
        
        builder.HasMany(instance => instance.Groups)
            .WithMany(group => group.Instances)
            .UsingEntity(joinEntityName: "instanceGroups",
                configureRight: l => l.HasOne(typeof(Group)).WithMany().HasForeignKey("groupId").HasPrincipalKey(nameof(Group.Id)),
                configureLeft: r => r.HasOne(typeof(Instance)).WithMany().HasForeignKey("instanceId").HasPrincipalKey(nameof(Instance.Id)),
                configureJoinEntityType: j => j.HasKey("groupId", "instanceId"));
    }
}