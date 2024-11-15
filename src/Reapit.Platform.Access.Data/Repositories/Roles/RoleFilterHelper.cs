using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.Repositories.Roles;

/// <summary>Filter helper for <see cref="Role"/> queries.</summary>
public static class RoleFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of <see cref="Role"/>.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyCursorFilter(this IQueryable<Role> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(user => user.Cursor > value);

    /// <summary>Filters a collection of <see cref="Role"/> objects by <see cref="User"/> association.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyUserIdFilter(this IQueryable<Role> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.Users.Any(user => user.Id == value));
    
    /// <summary>Filters a collection of <see cref="Role"/> objects by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyNameFilter(this IQueryable<Role> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.Name == value);
    
    /// <summary>Filters a collection of <see cref="Role"/> objects by description.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyDescriptionFilter(this IQueryable<Role> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.Description.Contains(value));
    
    /// <summary>Filters a collection of <see cref="Role" /> objects by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyCreatedFromFilter(this IQueryable<Role> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Role" /> objects by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyCreatedToFilter(this IQueryable<Role> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateCreated < value.Value);
    
    /// <summary>Filters a collection of <see cref="Role" /> objects by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyModifiedFromFilter(this IQueryable<Role> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateModified >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Role" /> objects by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Role> ApplyModifiedToFilter(this IQueryable<Role> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(group => group.DateModified < value.Value);
}