using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of User entity.</summary>
public class User : RemoteEntityBase
{
    /// <summary>Initializes a new instance of the <see cref="User"/> class.</summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The name of the user.</param>
    /// <param name="email">The email address of the user.</param>
    public User(string id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        DateLastSynchronised = DateTimeOffsetProvider.Now;
    }

    /// <summary>Update the user.</summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="email">The email address of the user.</param>
    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
        DateLastSynchronised = DateTimeOffsetProvider.Now;
    }
    
    /// <summary>The unique identifier of the User in the organisations service.</summary>
    public string Id { get; private set; }
    
    /// <summary>The name of the User in the organisations service.</summary>
    public string Name { get; private set; }
    
    /// <summary>The email address of the User in the organisations service.</summary>
    public string Email { get; private set; }

    /// <summary>The user-organisation relationships associated with this user.</summary>
    public ICollection<OrganisationUser> OrganisationUsers { get; set; } = new List<OrganisationUser>();

    /// <summary>The user-role relationships associated with this user.</summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>The user-user group relationships associated with this user.</summary>
    public ICollection<UserGroupUser> UserGroupUsers { get; set; } = new List<UserGroupUser>();

    /// <inheritdoc /> 
    public override object AsSerializable()
        => new { Id, Name, Email, Sync = DateLastSynchronised };
}