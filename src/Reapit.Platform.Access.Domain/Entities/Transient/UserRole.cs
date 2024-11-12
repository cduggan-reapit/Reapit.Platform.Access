using System.ComponentModel.DataAnnotations.Schema;

namespace Reapit.Platform.Access.Domain.Entities.Transient;

public class UserRole
{
    public string UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    public string RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; }
}