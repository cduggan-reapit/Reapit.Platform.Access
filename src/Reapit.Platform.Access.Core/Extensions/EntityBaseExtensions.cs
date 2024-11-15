using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Core.Extensions;

public static class EntityBaseExtensions
{
    /// <summary>Gets the maximum cursor value from a collection of <typeparamref name="TEntity"/>.</summary>
    /// <param name="set">The collection.</param>
    /// <returns>The maximum Cursor value from the collection if it contains any items; otherwise zero.</returns>
    public static long GetMaximumCursor<TEntity>(this IEnumerable<TEntity> set)
        where TEntity: EntityBase
    {
        var list = set.ToList();
        return list.Any()
            ? list.Max(item => item.Cursor)
            : 0;
    }
}