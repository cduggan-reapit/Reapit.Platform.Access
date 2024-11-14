using Reapit.Platform.Access.Domain.Entities.Abstract;
using Reapit.Platform.Access.Domain.Entities.Transient;

namespace Reapit.Platform.Access.Domain.Entities;

public class Instance : EntityBase
{
    public Instance(string id) : base(id)
    {
    }
    
    public string Name { get; private set; }

    public string ProductId { get; set; }
    public Product Product { get; private set; }
    
    public string OrganisationId { get; set; }
    public Organisation Organisation { get; set; }
    
    public override object AsSerializable()
    {
        throw new NotImplementedException();
    }
}