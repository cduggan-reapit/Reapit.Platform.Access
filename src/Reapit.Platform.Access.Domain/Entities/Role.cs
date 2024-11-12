using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Domain.Entities;

public class Role : EntityBase
{
    public Role(string id, string name) 
        : base(id)
    {
        Name = name;
    }
    
    public string Name { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; }

    public override object AsSerializable()
        => this;
}