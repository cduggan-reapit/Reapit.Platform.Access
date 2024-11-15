using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;

/// <summary>Request for a collection of roles.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="UserId">Limit results to roles assigned to the user with this unique identifier.</param>
/// <param name="Name">Limit results to roles matching this name.</param>
/// <param name="Description">Limit results to roles matching this name.</param>
/// <param name="CreatedFrom">Limit results to roles created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to roles created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to roles last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to roles last modified before this date.</param>
public record GetRolesQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? UserId = null,
    string? Name = null,
    string? Description = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null) 
    : IRequest<IEnumerable<Role>>;