using Reapit.Platform.Access.Domain.Entities.Abstract;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of the Organisation entity.</summary>
public class Organisation : RemoteEntityBase
{
    /// <summary>The unique identifier of the organisation in the organisations service.</summary>
    public required string Id { get; set; }
    
    /// <summary>The name of the organisation in the organisations service.</summary>
    public required string Name { get; set; }

    /// <summary>The product instances associated with the organisation.</summary>
    public ICollection<Instance> Instances { get; set; } = new List<Instance>();
    
    /// <summary>The user groups associated with the organisation.</summary>
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public ICollection<OrganisationUser> OrganisationUsers { get; set; } = new List<OrganisationUser>();
}