using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class InstanceConfiguration : IEntityTypeConfiguration<Instance>
{
    public void Configure(EntityTypeBuilder<Instance> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("instances");
        
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
    }
}