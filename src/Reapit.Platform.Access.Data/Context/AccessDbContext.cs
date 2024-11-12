using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Context;

/// <summary>Represents a session with the access management database.</summary>
/// <param name="options">The options for this context.</param>
public class AccessDbContext(DbContextOptions<AccessDbContext> options) : DbContext(options)
{
    /// <summary>The collection of users.</summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>The collection of user-organisation relationships.</summary>
    public DbSet<OrganisationUser> OrganisationUsers { get; set; }
    
    /// <summary>The collection of organisations.</summary>
    public DbSet<Organisation> Organisations { get; set; }

    /// <summary>Configures the Entity Framework model.</summary>
    /// <param name="builder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(typeof(AccessDbContext).Assembly);
}