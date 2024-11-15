using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Groups;

/// <summary>Repository for managing <see cref="Group"/> entities. </summary>
public interface IGroupRepository : IBaseRepository<Group>
{
    /// <summary>Get a collection of groups with optional filters applied.</summary>
    /// <param name="userId">Limit results to groups associated with the user with this unique identifier.</param>
    /// <param name="organisationId">Limit results to groups associated with the organisation with this unique identifier.</param>
    /// <param name="name">Limit results to groups matching this name.</param>
    /// <param name="description">Limit results to groups matching this description.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to groups matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Group"/> objects.</returns>
    // TODO: consider creating a data-layer object (or objects) containing these values.  There are too many parameters here.
    Task<IEnumerable<Group>> GetGroupsAsync(
        string? userId = null,
        string? organisationId = null,
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>Get a group by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken);
}