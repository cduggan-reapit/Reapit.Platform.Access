namespace Reapit.Platform.Access.Domain.Entities;

/// <summary>Representation of a product to which access is managed.</summary>
public class Product : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Product"/> class.</summary>
    /// <param name="name">The name of the product</param>
    public Product(string name)
        => Name = name;

    /// <summary>Update the product.</summary>
    /// <param name="name">The name of the product.</param>
    public void Update(string? name)
        => Name = GetUpdateValue(Name, name);
    
    // Add instance
    public void AddInstance(Instance instance)
        => Instances.Add(instance);
    
    /// <summary>The name of the product.</summary>
    public string Name { get; private set; }

    /// <summary>The instances of the product.</summary>
    public ICollection<Instance> Instances { get; } = new List<Instance>();

    /// <inheritdoc />
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
}
