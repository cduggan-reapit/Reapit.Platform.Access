﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Access.Data.Context.Configuration.Helpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("roles");
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
    }
}