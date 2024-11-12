using Reapit.Platform.Access.Data.Context;

namespace Reapit.Platform.Access.Data.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccessDbContext _context;

    // private IDummyRepository? _dummyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(AccessDbContext context)
        => _context = context;
    
    // /// <inheritdoc />
    // public IDummyRepository Dummies 
    //     => _dummyRepository ??= new DummyRepository(_context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}