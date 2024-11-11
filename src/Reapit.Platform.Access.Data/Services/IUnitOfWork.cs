using Reapit.Platform.Access.Data.Repositories;

namespace Reapit.Platform.Access.Data.Services;

/// <summary>Service managing database transactions.</summary>
public interface IUnitOfWork
{
    /// <summary>The dummy repository.</summary>
    public IDummyRepository Dummies { get; }
    
    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}