using System.ComponentModel.DataAnnotations.Schema;

namespace Reapit.Platform.Access.Domain.Entities.Transient;

public class UserGroupUser
{
    public string UserGroupId { get; set; }
    
    [ForeignKey(nameof(UserGroupId))]
    public UserGroup UserGroup { get; set; }
    
    public long OrganisationUserId { get; set; }
    
    [ForeignKey(nameof(OrganisationUserId))]
    public OrganisationUser OrganisationUser { get; set; }
}