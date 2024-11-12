using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of User entity.</summary>
public class User : RemoteEntityBase
{
    /// <summary>The unique identifier of the User in the organisations service.</summary>
    public required string Id { get; set; }
    
    /// <summary>The name of the User in the organisations service.</summary>
    public required string Name { get; set; }

    /// <summary>The user-organisation relationships associated with this user.</summary>
    public ICollection<OrganisationUser> OrganisationUsers { get; set; } = new List<OrganisationUser>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}