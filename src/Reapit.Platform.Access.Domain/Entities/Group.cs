namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents a group with which users can be associated.</summary>
public class Group : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Group"/> class.</summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="description">A description of the group.</param>
    /// <param name="organisationId">The unique identifier of the organisation with which the group is associated.</param>
    public Group(string name, string? description, string organisationId)
    {
        Name = name;
        Description = description;
        OrganisationId = organisationId;
    }

    /// <summary>Update the properties of the group.</summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="description">A description of the group.</param>
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
    
    /// <summary>The name of the group.</summary>
    public string Name { get; private set; }
    
    /// <summary>A description of the group.</summary>
    public string? Description { get; private set; }
    
    /// <summary>The unique identifier of the organisation with which the group is associated.</summary>
    public string OrganisationId { get; init; }
    
    /// <summary>The organisation with which the group is associated.</summary>
    public Organisation? Organisation { get; init; }

    /// <summary>The collection of users associated with this group.</summary>
    public ICollection<User> Users { get; init; } = new List<User>();

    /// <summary>The product instances that the group can access.</summary>
    public IEnumerable<Instance> Instances { get; } = new List<Instance>();

    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, Description, OrganisationId, DateCreated, DateModified };
}