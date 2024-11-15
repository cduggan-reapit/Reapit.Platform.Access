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

        var now = DateTimeOffsetProvider.Now;
        Cursor = (long)(now - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        DateLastSynchronised = now;
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

    /// <summary>Determines whether the user is a member of a given organisation.</summary>
    /// <param name="organisationId">The unique identifier of the organisation.</param>
    public bool IsMemberOfOrganisation(string organisationId) 
        => Organisations.Any(o => o.Id == organisationId);

    /// <summary>The unique identifier of the User in the organisations service.</summary>
    public string Id { get; init; }
    
    /// <summary>The name of the User in the organisations service.</summary>
    public string Name { get; private set; }
    
    /// <summary>The email address of the User in the organisations service.</summary>
    public string Email { get; private set; }
    
    /// <summary>The cursor used for paging the user collection.</summary>
    public long Cursor { get; private init; }

    /// <summary>The organisations that this user is associated with.</summary>
    public ICollection<Organisation> Organisations { get; init; } = new List<Organisation>();

    /// <summary>The collection of groups with which the user is associated.</summary>
    public IEnumerable<Group> Groups { get; } = new List<Group>();

    /// <summary>The collection of roles assigned to the user.</summary>
    public IEnumerable<Role> Roles { get; } = new List<Role>();
    
    /// <inheritdoc /> 
    public override object AsSerializable()
        => new { Id, Name, Email, Sync = DateLastSynchronised.UtcDateTime };
}