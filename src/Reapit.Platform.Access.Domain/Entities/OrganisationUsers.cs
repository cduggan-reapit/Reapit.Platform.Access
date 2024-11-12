using System.ComponentModel.DataAnnotations.Schema;
using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents the relationship between a user and an organisation.</summary>
public class OrganisationUser : RemoteEntityBase
{
    /// <summary>Unique identifier for an organisation-user relationship.</summary>
    public long Id { get; init; }
    
    /// <summary>The unique identifier of the user.</summary>
    public required string UserId { get; init; }
    
    /// <summary>The unique identifier of the organisation.</summary>
    public required string OrganisationId { get; init; }
    
    /// <summary>The user with which this entity is associated.</summary>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    /// <summary>The organisation with which this entity is associated.</summary>
    [ForeignKey(nameof(OrganisationId))]
    public Organisation? Organisation { get; set; }
    
    public ICollection<UserGroupUser> UserGroupUsers { get; set; } = new List<UserGroupUser>();
}