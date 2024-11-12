using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Domain.Entities;

public class UserGroup : EntityBase
{
    
    public UserGroup(string id, string name, string organisationId) 
        : base(id)
    {
        Name = name;
        OrganisationId = organisationId;
    }
    
    public string Name { get; private set; }
    
    public string OrganisationId { get; private init; }
    
    public Organisation Organisation { get; init; }

    public ICollection<UserGroupUser> UserGroupUsers { get; set; } = new List<UserGroupUser>();
    
    public ICollection<InstanceUserGroup> InstanceUserGroups { get; set; } = new List<InstanceUserGroup>();

    public override object AsSerializable()
        => this;
}