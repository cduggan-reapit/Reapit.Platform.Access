using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Repositories.Users;

namespace Reapit.Platform.Access.Data.Services;

/// <summary>Service managing database transactions.</summary>
public interface IUnitOfWork
{
    /// <summary>The user repository.</summary>
    public IUserRepository Users { get; }
    
    /// <summary>The organisation repository.</summary>
    public IOrganisationRepository Organisations { get; }
    
    
    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}