using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;

namespace Reapit.Platform.Access.Data.UnitTests.Context;

public class TestDbContextFactory : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");
    
    public AccessDbContext CreateContext(bool ensureCreated = true)
    {
        _connection.Open();
        var context = InstantiateDbContext();
        
        if (!ensureCreated) 
            return context;
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public async Task<AccessDbContext> CreateContextAsync(
        bool ensureCreated = true, 
        CancellationToken cancellationToken = default)
    {
        await _connection.OpenAsync(cancellationToken);
        var context = InstantiateDbContext();

        if (!ensureCreated) 
            return context;
        
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
        return context;
    }
    
    public void Dispose()
        => _connection.Dispose();

    public async ValueTask DisposeAsync()
        => await _connection.DisposeAsync();

    private AccessDbContext InstantiateDbContext()
        => new(new DbContextOptionsBuilder<AccessDbContext>().UseSqlite(_connection).Options);
}