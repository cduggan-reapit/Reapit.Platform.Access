using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Groups;

/// <summary>Repository for managing <see cref="Group"/> entities. </summary>
public interface IGroupRepository : IBaseRepository<Group>
{
    /// <summary>Get a collection of groups with optional filters applied.</summary>
    /// <param name="cursor">The offset cursor; default 0.</param>
    /// <param name="pageSize">The maximum number of results to return; default 25.</param>
    /// <param name="userId">Limit results to groups associated with the user with this unique identifier.</param>
    /// <param name="organisationId">Limit results to groups associated with the organisation with this unique identifier.</param>
    /// <param name="name">Limit results to groups matching this name.</param>
    /// <param name="description">Limit results to groups matching this description.</param>
    /// <param name="createdFrom">Limit results to groups created on or after this date (UTC).</param>
    /// <param name="createdTo">Limit results to groups created before this date (UTC).</param>
    /// <param name="modifiedFrom">Limit results to groups last modified on or after this date.</param>
    /// <param name="modifiedTo">Limit results to groups last modified before this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Group"/> objects.</returns>
    // TODO: consider creating a data-layer object (or objects) containing these values.  There are too many parameters here.
    Task<IEnumerable<Group>> GetGroupsAsync(
        long? cursor = null,
        int pageSize = 25,
        string? userId = null,
        string? organisationId = null,
        string? name = null,
        string? description = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>Get a group by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken);
}