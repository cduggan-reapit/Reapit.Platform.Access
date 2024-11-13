using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Groups;

/// <summary>Filter helper for <see cref="Group"/> queries.</summary>
public static class GroupFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of <see cref="Group"/>.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyCursorFilter(this IQueryable<Group> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(user => user.Cursor > value);

    /// <summary>Filters a collection of <see cref="Group"/> objects by <see cref="User"/> association.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyUserIdFilter(this IQueryable<Group> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.GroupUsers.Any(ou => ou.OrganisationUser.UserId == value));
    
    /// <summary>Filters a collection of <see cref="Group"/> objects by <see cref="Organisation"/> association.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyOrganisationIdFilter(this IQueryable<Group> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.OrganisationId == value);
    
    /// <summary>Filters a collection of <see cref="Group"/> objects by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyNameFilter(this IQueryable<Group> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.Name == value);
    
    /// <summary>Filters a collection of <see cref="Group"/> objects by description.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyDescriptionFilter(this IQueryable<Group> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.Description.Contains(value));
    
    /// <summary>Filters a collection of <see cref="Group" /> objects by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyCreatedFromFilter(this IQueryable<Group> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Group" /> objects by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyCreatedToFilter(this IQueryable<Group> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateCreated < value.Value);
    
    /// <summary>Filters a collection of <see cref="Group" /> objects by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyModifiedFromFilter(this IQueryable<Group> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateModified >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Group" /> objects by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Group> ApplyModifiedToFilter(this IQueryable<Group> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateModified < value.Value);
}