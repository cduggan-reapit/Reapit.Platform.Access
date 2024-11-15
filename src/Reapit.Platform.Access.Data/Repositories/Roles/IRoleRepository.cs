using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Roles;

/// <summary>Repository for managing <see cref="Role"/> entities. </summary>
public interface IRoleRepository : IBaseRepository<Role>
{
    /// <summary>Get a collection of roles with optional filters applied.</summary>
    /// <param name="userId">Limit results to roles assigned to the user with this unique identifier.</param>
    /// <param name="name">Limit results to roles matching this name.</param>
    /// <param name="description">Limit results to roles matching this description.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to roles matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Role"/> objects.</returns>
    Task<IEnumerable<Role>> GetRolesAsync(
        string? userId = null,
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
}