using System.Text.Json;
using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities;

public class InstanceTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_WhenInitializingInstance()
    {
        var fixedId = Guid.NewGuid();
        var fixedDate = new DateTimeOffset(2024, 11, 13, 13, 40, 11, TimeSpan.FromHours(1));

        using var identifierContext = new GuidProviderContext(fixedId);
        using var timeContext = new DateTimeOffsetProviderContext(fixedDate);
        
        const string name = "this is the name";
        const string productId = "this is the product id";
        const string organisationId = "this is the organisation id";
        
        var expectedEpochTime = (long)(fixedDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;

        var sut = new Instance(name, productId, organisationId);

        // Explicit
        sut.Name.Should().Be(name);
        sut.ProductId.Should().Be(productId);
        sut.OrganisationId.Should().Be(organisationId);
        
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
        var sut = new Instance("name", "productId", "organisationId");
        sut.Update(null);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenValuesUnchanged()
    {
        var sut = new Instance("name", "productId", "organisationId");
        sut.Update(sut.Name);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }

    [Fact]
    public void Update_UpdatesEntity_WhenValuesChanged()
    {
        var sut = new Instance("name", "productId", "organisationId");
        sut.Update("new name");

        sut.IsDirty.Should().BeTrue();
        sut.DateModified.Should().NotBe(sut.DateCreated);
    }
    
    /*
     * AddGroup
     */

    [Fact]
    public void AddGroup_AddsGroupToCollection()
    {
        var group = new Group("name", "description", "organisationId");
        var sut = new Instance("name", "productId", "organisationId");
        sut.AddGroup(group);
        sut.Groups.Should().BeEquivalentTo([group]);
    }
    
    /*
     * RemoveGroup
     */
    
    [Fact]
    public void RemoveGroup_RemovesGroupFromCollection()
    {
        var group = new Group("name", "description", "organisationId");
        var sut = new Instance("name", "productId", "organisationId")
        {
            Groups = [group]
        };
        
        sut.RemoveGroup(group);
        sut.Groups.Should().BeEmpty();
    }
    
    /*
     * SoftDelete
     */
    
    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        using var timeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var sut = new Instance( "name", "productId", "organisationId");
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
    public void AsSerializable_ReturnsAnonymousObject_ForInstance()
    {
        const string name = "name";
        
        var instance = new Instance(name, "productId", "organisationId");
        var expected = new { instance.Id, instance.Name, instance.ProductId, instance.OrganisationId, instance.DateCreated, instance.DateModified };
        
        var actual = instance.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_ForEntity()
    {
        var entity = new Instance("name", "productId", "organisationId");
        var expected = JsonSerializer.Serialize(entity.AsSerializable());
        var actual = entity.ToString();
        actual.Should().Be(expected);
    }
}