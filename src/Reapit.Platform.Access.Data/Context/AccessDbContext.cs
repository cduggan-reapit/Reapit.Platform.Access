﻿using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context;

/// <summary>Represents a session with the access management database.</summary>
/// <param name="options">The options for this context.</param>
public class AccessDbContext(DbContextOptions<AccessDbContext> options) : DbContext(options)
{
    /// <summary>The collection of user groups.</summary>
    public DbSet<Group> Groups { get; init; }
    
    /// <summary>The collection of user roles.</summary>
    public DbSet<Role> Roles { get; init; }
    
    /// <summary>The collection of users.</summary>
    public DbSet<User> Users { get; init; }
    
    /// <summary>The collection of organisations.</summary>
    public DbSet<Organisation> Organisations { get; init; }
    
    /// <summary>Configures the Entity Framework model.</summary>
    /// <param name="builder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(typeof(AccessDbContext).Assembly);
}