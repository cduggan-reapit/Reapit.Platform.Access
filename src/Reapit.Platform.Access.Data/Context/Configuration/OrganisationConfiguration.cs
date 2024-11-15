﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Organisation"/> type.</summary>
[ExcludeFromCodeCoverage]
public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ConfigureRemoteEntityBase()
            .ToTable("organisations");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasMaxLength(100);

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.HasMany(group => group.Users)
            .WithMany(user => user.Organisations)
            .UsingEntity(joinEntityName: "organisationUsers",
                configureLeft: l => l.HasOne(typeof(Organisation)).WithMany().HasForeignKey("organisationId").HasPrincipalKey(nameof(Organisation.Id)),
                configureRight: r => r.HasOne(typeof(User)).WithMany().HasForeignKey("userId").HasPrincipalKey(nameof(User.Id)),
                configureJoinEntityType: j => j.HasKey("organisationId", "userId"));
    }
}