using Microsoft.EntityFrameworkCore;

namespace Reapit.Platform.Access.Data.Context;

public class AccessDbContext : DbContext
{
    // public DbSet<Dummy> Dummies { get; set; }

    public AccessDbContext(DbContextOptions<AccessDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(typeof(AccessDbContext).Assembly);
}