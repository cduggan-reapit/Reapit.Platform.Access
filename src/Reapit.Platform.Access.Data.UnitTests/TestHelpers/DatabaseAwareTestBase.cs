using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.UnitTests.Context;

namespace Reapit.Platform.Access.Data.UnitTests.TestHelpers;

public abstract class DatabaseAwareTestBase
{
    private readonly TestDbContextFactory _contextFactory = new();
    private AccessDbContext? _context;

    public AccessDbContext GetContext(bool ensureCreated = true)
        => _context ??= _contextFactory.CreateContext(ensureCreated);
    
    public async Task<AccessDbContext> GetContextAsync(bool ensureCreated = true, CancellationToken cancellationToken = default)
        => _context ??= await _contextFactory.CreateContextAsync(ensureCreated, cancellationToken);
}