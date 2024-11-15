using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities;

public class RoleTests
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
        const string description = "this is the description";
        const string organisationId = "this is the organisation identifier";
        
        var expectedEpochTime = (long)(fixedDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;

        var sut = new Role(name, description);

        // Explicit
        sut.Name.Should().Be(name);
        sut.Description.Should().Be(description);
        
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
        var sut = new Role("name", "description");
        sut.Update(null, null);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenValuesUnchanged()
    {
        var sut = new Role("name", "description");
        sut.Update(sut.Name, sut.Description);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }

    [Fact]
    public void Update_UpdatesEntity_WhenValuesChanged()
    {
        var sut = new Role("name", "description");
        sut.Update("new name", sut.Description);

        sut.IsDirty.Should().BeTrue();
        sut.DateModified.Should().NotBe(sut.DateCreated);
    }

    [Fact]
    public void Update_UpdatesEntity_WhenCurrentValueNull()
    {
        var sut = new Role("name", null);
        sut.Update(null, "description");

        sut.IsDirty.Should().BeTrue();
        sut.DateModified.Should().NotBe(sut.DateCreated);
    }   
    
    /*
     * AddUser
     */

    [Fact]
    public void AddUser_AddsUserToCollection()
    {
        var user = new User("user-id", "user-name", "user-email");
        var sut = new Role("name", "description");
        sut.AddUser(user);
        sut.Users.Should().BeEquivalentTo([user]);
    }
    
    /*
     * RemoveUser
     */
    
    [Fact]
    public void RemoveUser_RemovesUserFromCollection()
    {
        var user = new User("user-id", "user-name", "user-email");
        var sut = new Role("name", "description")
        {
            Users = [user]
        };
        
        sut.RemoveUser(user);
        sut.Users.Should().BeEmpty();
    }
    
    /*
     * SoftDelete
     */
    
    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        using var timeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var sut = new Role( "name", "description");
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
    public void AsSerializable_ReturnsAnonymousObject_ForUser()
    {
        const string name = "name";
        
        var role = new Role(name, null);
        var expected = new { role.Id, role.Name, role.DateCreated, role.DateModified };
        
        var actual = role.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
}