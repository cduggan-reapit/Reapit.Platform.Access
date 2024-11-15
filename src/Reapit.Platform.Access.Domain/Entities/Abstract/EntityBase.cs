using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.Entities.Abstract;

public abstract class EntityBase
{
    /// <summary>Initialize a new instance of the <see cref="EntityBase"/> class.</summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected EntityBase(string id)
    {
        Id = id;
        SetDateCreated();
    }
    
    /// <summary>The unique identifier of the entity.</summary>
    public string Id { get; protected init; }

    /// <summary>The cursor hash for this entity.</summary>
    public long Cursor { get; private set; }

    /// <summary>The UTC date and time on which the entity was created.</summary>
    public DateTime DateCreated { get; set; }

    /// <summary>The UTC date and time on which the entity was last modified.</summary>
    public DateTime DateModified { get; set; }
    
    /// <summary>The UTC date and time on which the entity was deleted.</summary>
    public DateTime? DateDeleted { get; private set; }
    
    /// <summary>Flag indicating whether the entity has been modified.</summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Method to determine the value of a field in an update operation, setting the last modified date and dirty flag
    /// if the value should be changed. 
    /// </summary>
    /// <param name="current">The current value of the field.</param>
    /// <param name="updated">The proposed value of the field.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <returns>The new value when `proposed` is not equal to `current`, otherwise current.</returns>
    internal T GetUpdateValue<T>(T current, T? updated)
    {
        // No matter what, return current if updated is null 
        if (updated == null)
            return current;
        
        // If current is null and updated is not null, return updated.
        if (current == null)
        {
            IsDirty = true;
            SetDateModified();
            return updated;
        }
        
        // Return current if both have values and the values are equal.
        if (current.Equals(updated))
            return current;
        
        // Return updated if both have values and the values are not equal. 
        IsDirty = true;
        SetDateModified();
        return updated;
    }

    /// <summary>Set the creation date to the current timestamp.</summary>
    private void SetDateCreated()
    {
        // Get the date once so all the two date fields get the same value
        var created = DateTimeOffsetProvider.Now;
        
        DateCreated = created.UtcDateTime;
        DateModified = created.UtcDateTime;
        
        // And the cursor is the unix epoch in microseconds
        Cursor = (long)(created - DateTimeOffset.UnixEpoch).TotalMicroseconds;
    }

    /// <summary>Set the modified date to the current timestamp.</summary>
    private void SetDateModified() 
        => DateModified = DateTimeOffsetProvider.Now.UtcDateTime;

    /// <summary>Marks the entity as deleted.</summary>
    public void SoftDelete()
        => DateDeleted = DateTimeOffsetProvider.Now.UtcDateTime;
    
    /// <summary>Gets an anonymous, serializable object representing this entity.</summary>
    /// <remarks>
    /// Due to relationships between subclasses of <see cref="EntityBase"/>, circular relationships can be established
    /// which causes problems for object serialization.  This method generates an object which is safe to serialize.
    /// </remarks>
    public abstract object AsSerializable();
}