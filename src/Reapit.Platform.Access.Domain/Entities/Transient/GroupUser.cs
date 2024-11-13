using System.ComponentModel.DataAnnotations.Schema;

namespace Reapit.Platform.Access.Domain.Entities.Transient;

public class GroupUser
{
    public GroupUser()
    {
    }

    public GroupUser(Group group, OrganisationUser membership)
    {
        Group = group;
        GroupId = group.Id;
        
        OrganisationUser = membership;
        OrganisationUserId = membership.Id;
    }
    
    /// <summary>The unique identifier of the group.</summary>
    public string GroupId { get; init; }
    
    /// <summary>The group.</summary>
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; init; }
    
    /// <summary>The unique identifier of the organisation-user.</summary>
    public long OrganisationUserId { get; init; }
    
    /// <summary>The organisation-user.</summary>
    [ForeignKey(nameof(OrganisationUserId))]
    public OrganisationUser OrganisationUser { get; init; }
}