using System.ComponentModel.DataAnnotations.Schema;

namespace Reapit.Platform.Access.Domain.Entities.Transient;

public class InstanceUserGroup
{
    /// <summary>The unique identifier of the instance.</summary>
    public string InstanceId { get; set; }
    
    [ForeignKey(nameof(InstanceId))]
    public Instance Instance { get; set; }
    
    /// <summary>The unique identifier of the user group.</summary>
    public string UserGroupId { get; set; }
    
    [ForeignKey(nameof(UserGroupId))]
    public Group Group { get; set; }
}