using System.ComponentModel.DataAnnotations.Schema;
using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;
using Reapit.Platform.Common.Providers.Identifiers;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents a group with which users can be associated.</summary>
public class Group : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Group"/> class.</summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="description">A description of the group.</param>
    /// <param name="organisationId">The unique identifier of the organisation with which the group is associated.</param>
    public Group(string name, string description, string organisationId) 
        : base(GuidProvider.New.ToString("N"))
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
    
    /// <summary>The name of the group.</summary>
    public string Name { get; private set; }
    
    /// <summary>A description of the group.</summary>
    public string Description { get; private set; }
    
    /// <summary>The unique identifier of the organisation with which the group is associated.</summary>
    public string OrganisationId { get; init; }
    
    /// <summary>The organisation with which the group is associated.</summary>
    [ForeignKey(nameof(OrganisationId))]
    public Organisation? Organisation { get; init; }

    /// <summary>The collection of group-to-organisation-user relationships associated with this group.</summary>
    public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
    
    /// <summary>The collection of product instances that this group is able to access.</summary>
    public ICollection<InstanceUserGroup> InstanceUserGroups { get; set; } = new List<InstanceUserGroup>();

    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, Description, OrganisationId, DateCreated, DateModified };
}