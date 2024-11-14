using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of the Organisation entity.</summary>
public class Organisation : RemoteEntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Organisation"/> class.</summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <param name="name">The name of the organisation.</param>
    public Organisation(string id, string name)
    {
        Id = id;
        Name = name;
        DateLastSynchronised = DateTimeOffsetProvider.Now;
    }

    /// <summary>Update the organisation instance.</summary>
    /// <param name="name">The name of the organisation.</param>
    public void Update(string name)
    {
        Name = name;
        DateLastSynchronised = DateTimeOffsetProvider.Now;
    }

    /// <summary>Add a user to the organisation users collection.</summary>
    /// <param name="user">The user to add.</param>
    public void AddUser(User user)
        => Users.Add(user);
    
    /// <summary>Remove a user from the organisation users collection.</summary>
    /// <param name="user">The user to remove.</param>
    public void RemoveUser(User user)
        => Users.Remove(user);
    
    /// <summary>The unique identifier of the organisation in the organisations service.</summary>
    public string Id { get; init; }
    
    /// <summary>The name of the organisation in the organisations service.</summary>
    public string Name { get; private set; }

    /// <summary>The product instances associated with the organisation.</summary>
    public ICollection<Instance> Instances { get; set; } = new List<Instance>();
    
    /// <summary>The user groups associated with the organisation.</summary>
    public ICollection<Group> Groups { get; set; } = new List<Group>();

    /// <summary>The users associated with this organisation.</summary>
    public ICollection<User> Users { get; set; } = new List<User>();
    
    /// <inheritdoc /> 
    public override object AsSerializable()
        => new { Id, Name, Sync = DateLastSynchronised.UtcDateTime };
}