using System.ComponentModel.DataAnnotations.Schema;
using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents the relationship between a user and an organisation.</summary>
public class OrganisationUser : RemoteEntityBase
{
    /// <summary>Initializes a new instance of the <see cref="OrganisationUser"/> class.</summary>
    public OrganisationUser()
    {
    }
    
    /// <summary>Initializes a new instance of the <see cref="OrganisationUser"/> class.</summary>
    /// <param name="organisation">The organisation.</param>
    /// <param name="user">The user.</param>
    public OrganisationUser(Organisation organisation, User user)
    {
        OrganisationId = organisation.Id;
        Organisation = organisation;
        UserId = user.Id;
        User = user;
        DateLastSynchronised = DateTimeOffsetProvider.Now;
    }
    
    /// <summary>Unique identifier for an organisation-user relationship.</summary>
    public long Id { get; init; }
    
    /// <summary>The unique identifier of the user.</summary>
    public string UserId { get; init; }
    
    /// <summary>The unique identifier of the organisation.</summary>
    public string OrganisationId { get; init; }
    
    /// <summary>The user with which this entity is associated.</summary>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    /// <summary>The organisation with which this entity is associated.</summary>
    [ForeignKey(nameof(OrganisationId))]
    public Organisation? Organisation { get; set; }
    
    /// <inheritdoc /> 
    public override object AsSerializable()
        => new { Id, User = UserId, Organisation = OrganisationId, Sync = DateLastSynchronised.UtcDateTime };
}