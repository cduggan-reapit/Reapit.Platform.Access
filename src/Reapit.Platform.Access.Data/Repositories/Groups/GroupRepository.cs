using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Groups;

// <inheritdoc/>
public class GroupRepository(AccessDbContext context) : BaseRepository<Group>(context), IGroupRepository
{
    // <inheritdoc/>
    public async Task<IEnumerable<Group>> GetGroupsAsync(
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
        CancellationToken cancellationToken = default)
        => await context.Groups
            .ApplyCursorFilter(cursor)
            .ApplyUserIdFilter(userId)
            .ApplyOrganisationIdFilter(organisationId)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyCreatedFromFilter(createdFrom)
            .ApplyCreatedToFilter(createdTo)
            .ApplyModifiedFromFilter(modifiedFrom)
            .ApplyModifiedToFilter(modifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Groups
            .Include(group => group.Users)
            .SingleOrDefaultAsync(group => group.Id  == id, cancellationToken);
}