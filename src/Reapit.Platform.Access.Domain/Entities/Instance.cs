namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Represents an instance of a product.</summary>
public class Instance : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Instance"/> class.</summary>
    /// <param name="name">The name of the instance.</param>
    /// <param name="productId">The unique identifier of the product that entity represents and instance of.</param>
    /// <param name="organisationId">The unique identifier of the organisation with which the instance is associated.</param>
    public Instance(string name, string productId, string organisationId)
    {
        Name = name;
        OrganisationId = organisationId;
        ProductId = productId;
    }

    /// <summary>Update the instance.</summary>
    /// <param name="name">The name of the instance.</param>
    public void Update(string? name)
        => Name = GetUpdateValue(Name, name);

    /// <summary>Add a group to the instance.</summary>
    /// <param name="group">The group.</param>
    public void AddGroup(Group group)
        => Groups.Add(group);
    
    /// <summary>Remove a group from the instance.</summary>
    /// <param name="group">The group.</param>
    public void RemoveGroup(Group group)
        => Groups.Remove(group);

    /// <summary>The name of the instance.</summary>
    public string Name { get; private set; }

    /// <summary>The unique identifier of the product that the entity represents an instance of.</summary>
    public string ProductId { get; private init; }
    
    /// <summary>The product that the entity represents an instance of.</summary>
    public Product? Product { get; private init; }
    
    /// <summary>The unique identifier of the organisation with which this instance is associated.</summary>
    public string OrganisationId { get; private init; }
    
    /// <summary>The organisation with which this instance is associated.</summary>
    public Organisation? Organisation { get; private init; }

    /// <summary>The groups which have been granted access to this instance.</summary>
    public ICollection<Group> Groups { get; init; } = new List<Group>();

    /// <inheritdoc />
    public override object AsSerializable()
        => new { Id, Name, ProductId, OrganisationId, DateCreated, DateModified };
}