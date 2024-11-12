using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of the Organisation entity.</summary>
public class Organisation : RemoteEntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Organisation"/> class.</summary>
    public Organisation()
    {
    }
    
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
    
    /// <summary>The unique identifier of the organisation in the organisations service.</summary>
    public string Id { get; init; }
    
    /// <summary>The name of the organisation in the organisations service.</summary>
    public string Name { get; private set; }

    /// <summary>The product instances associated with the organisation.</summary>
    public ICollection<Instance> Instances { get; set; } = new List<Instance>();
    
    /// <summary>The user groups associated with the organisation.</summary>
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public ICollection<OrganisationUser> OrganisationUsers { get; set; } = new List<OrganisationUser>();
    
    /// <inheritdoc /> 
    public override object AsSerializable()
        => new { Id, Name, Sync = DateLastSynchronised.UtcDateTime };
}