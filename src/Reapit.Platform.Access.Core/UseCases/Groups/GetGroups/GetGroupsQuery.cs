using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;

/// <summary>Request model used when fetching a collection of groups.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="UserId">Limit results to groups associated with the user with this unique identifier.</param>
/// <param name="OrganisationId">Limit results to groups associated with the organisation with this unique identifier.</param>
/// <param name="Name">Limit results to groups matching this name.</param>
/// <param name="Description">Limit results to groups matching this name.</param>
/// <param name="CreatedFrom">Limit results to groups created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to groups created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to groups last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to groups last modified before this date.</param>
public record GetGroupsQuery(
     long? Cursor = null,
     int PageSize = 25,
     string? UserId = null,
     string? OrganisationId = null,
     string? Name = null,
     string? Description = null,
     DateTime? CreatedFrom = null,
     DateTime? CreatedTo = null,
     DateTime? ModifiedFrom = null,
     DateTime? ModifiedTo = null)
    : IRequest<IEnumerable<Group>>;