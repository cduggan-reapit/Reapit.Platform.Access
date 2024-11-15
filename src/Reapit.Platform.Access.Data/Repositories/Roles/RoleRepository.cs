using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Roles;

// <inheritdoc/>
public class RoleRepository(AccessDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    // <inheritdoc/>
    public async Task<IEnumerable<Role>> GetRolesAsync(
        string? userId = null,
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null, 
        CancellationToken cancellationToken = default)
        => await context.Roles
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyUserIdFilter(userId)
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
    public async Task<Role?> GetRoleByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Roles
            .Include(role => role.Users)
            .SingleOrDefaultAsync(role => role.Id  == id, cancellationToken);
}