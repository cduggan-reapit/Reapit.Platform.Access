using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Common.Providers.Identifiers;

namespace Reapit.Platform.Access.Domain.Entities;

public class Product : EntityBase
{
    public Product(string name) 
        : base(GuidProvider.New.ToString("N"))
    {
        Name = name;
    }
    
    public string Name { get; private set; }
    
    public ICollection<Instance> Instances { get; set; }
    
    public override object AsSerializable()
        => this;
}