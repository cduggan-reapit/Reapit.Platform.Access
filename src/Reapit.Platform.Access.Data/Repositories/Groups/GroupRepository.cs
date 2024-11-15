using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Groups;

// <inheritdoc/>
public class GroupRepository(AccessDbContext context) : BaseRepository<Group>(context), IGroupRepository
{
    // <inheritdoc/>
    public async Task<IEnumerable<Group>> GetGroupsAsync(
        string? userId = null,
        string? organisationId = null,
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default)
        => await context.Groups
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyUserIdFilter(userId)
            .ApplyOrganisationIdFilter(organisationId)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Groups
            .Include(group => group.Users)
            .SingleOrDefaultAsync(group => group.Id  == id, cancellationToken);
}