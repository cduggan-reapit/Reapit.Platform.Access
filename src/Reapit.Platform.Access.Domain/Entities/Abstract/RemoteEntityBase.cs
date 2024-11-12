namespace Reapit.Platform.Access.Domain.Entities.Abstract;

/// <summary>Base class for entities maintained by external services (e.g. Organisations).</summary>
public abstract class RemoteEntityBase
{
    /// <summary>The date and time on which the record was last synchronised from the remote service.</summary>
    public DateTimeOffset DateLastSynchronised { get; set; }
    
    /// <summary>Method to determine the value of a field in an update operation.</summary>
    /// <param name="current">The current value of the field.</param>
    /// <param name="updated">The proposed value of the field.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <returns>The new value when `proposed` is not equal to `current`, otherwise current.</returns>
    internal T GetUpdateValue<T>(T current, T? updated)
        where T: notnull
    {
        // If current is equal to updated or updated is null, return current.
        if (current.Equals(updated) || updated == null)
            return current;

        return updated;
    }

    /// <summary>Get a serializable representation of the entity.</summary>
    public abstract object AsSerializable();
}