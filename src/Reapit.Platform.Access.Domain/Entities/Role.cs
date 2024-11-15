namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents a global role.</summary>
public class Role : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Role"/> class.</summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="description">A description of the role.</param>
    public Role(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    
    /// <summary>Update the properties of the role.</summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="description">A description of the role.</param>
    public void Update(string? name, string? description)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
    }

    /// <summary>Add a user to the members collection for this group.</summary>
    /// <param name="user">The user to add.</param>
    public void AddUser(User user)
        => Users.Add(user);

    /// <summary>Remove a user from the members collection for this group.</summary>
    /// <param name="user">The user to remove.</param>
    public void RemoveUser(User user)
        => Users.Remove(user);
    
    /// <summary>The name of the role.</summary>
    public string Name { get; private set; }
    
    /// <summary>A description of the role.</summary>
    public string? Description { get; private set; }

    /// <summary>The collection of users that have been assigned the role.</summary>
    public ICollection<User> Users { get; init; } = new List<User>();
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
}