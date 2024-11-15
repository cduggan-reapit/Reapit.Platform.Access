using System.Text.Json;
using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities;

public class ProductTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_WhenInitializing()
    {
        var fixedId = Guid.NewGuid();
        var fixedDate = new DateTimeOffset(2024, 11, 13, 13, 40, 11, TimeSpan.FromHours(1));

        using var identifierContext = new GuidProviderContext(fixedId);
        using var timeContext = new DateTimeOffsetProviderContext(fixedDate);
        
        const string name = "this is the name";
        
        var expectedEpochTime = (long)(fixedDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;

        var sut = new Product(name);

        // Explicit
        sut.Name.Should().Be(name);
        
        // Implicit
        sut.Id.Should().Be($"{fixedId:N}");
        sut.DateCreated.Should().Be(fixedDate.UtcDateTime);
        sut.DateModified.Should().Be(fixedDate.UtcDateTime);
        sut.Cursor.Should().Be(expectedEpochTime);
    }
    
    /*
     * Update
     */
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenParametersNull()
    {
        var sut = new Product("name");
        sut.Update(null);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenValuesUnchanged()
    {
        var sut = new Product("name");
        sut.Update(sut.Name);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }

    [Fact]
    public void Update_UpdatesEntity_WhenValuesChanged()
    {
        var sut = new Product("name");
        sut.Update("new name");

        sut.IsDirty.Should().BeTrue();
        sut.DateModified.Should().NotBe(sut.DateCreated);
    }
    
    /*
     * AddUser
     */

    [Fact]
    public void AddInstance_AddsInstanceToCollection()
    {
        var instance = new Instance("name", "productId", "organisationId");
        var sut = new Product("name");
        sut.AddInstance(instance);
        sut.Instances.Should().BeEquivalentTo([instance]);
    }
   
    /*
     * SoftDelete
     */
    
    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        using var timeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var sut = new Product( "name");
        sut.DateDeleted.Should().BeNull();
        
        var fixedDate = new DateTimeOffset(2024, 10, 18, 15, 12, 17, TimeSpan.FromHours(1));
        using var secondTimeFixture = new DateTimeOffsetProviderContext(fixedDate);

        sut.SoftDelete();

        sut.DateDeleted.Should().Be(fixedDate.UtcDateTime);
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_ReturnsAnonymousObject_ForProduct()
    {
        const string name = "name";
        
        var entity = new Product(name);
        var expected = new { entity.Id, entity.Name, entity.DateCreated, entity.DateModified };
        
        var actual = entity.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_ForEntity()
    {
        var entity = new Product("name");
        var expected = JsonSerializer.Serialize(entity.AsSerializable());
        var actual = entity.ToString();
        actual.Should().Be(expected);
    }
}