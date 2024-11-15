using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities;

public class OrganisationTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesEntity_FromParameters()
    {
        var timeFixture = new DateTimeOffset(2016, 4, 5, 9, 5, 16, TimeSpan.FromHours(-6));
        using var _ = new DateTimeOffsetProviderContext(timeFixture);
        const string id = "organisation-id", name = "name";

        var sut = new Organisation(id, name);
        sut.Id.Should().Be(id);
        sut.Name.Should().Be(name);
        sut.DateLastSynchronised.Should().Be(timeFixture);
    }
    
    /*
     * Update
     */

    [Fact]
    public void Update_UpdatesProperties_FromParameters()
    {
        var firstTimeFixture = new DateTimeOffset(2016, 4, 5, 9, 5, 16, TimeSpan.FromHours(-6));
        var secondTimeFixture = new DateTimeOffset(2024, 11, 12, 13, 14, 52, TimeSpan.Zero);
        using var firstContext = new DateTimeOffsetProviderContext(firstTimeFixture);
        const string id = "organisation-id", name = "name";

        var sut = new Organisation(id, "initial");
        sut.DateLastSynchronised.Should().Be(firstTimeFixture);
        
        using var secondContext = new DateTimeOffsetProviderContext(secondTimeFixture);
        sut.Update(name);
        sut.Id.Should().Be(id);
        sut.Name.Should().Be(name);
        sut.DateLastSynchronised.Should().Be(secondTimeFixture);
    }
    
    /*
     * AddUser
     */

    [Fact]
    public void AddUser_AddsUser_ToUserCollection()
    {
        var sut = new Organisation("id", "name");
        var user = new User("id", "name", "email");
        sut.AddUser(user);
        sut.Users.Should().HaveCount(1).And.AllSatisfy(actual => actual.Should().BeEquivalentTo(user));
    }
    
    /*
     * RemoveUser
     */
    
    [Fact]
    public void RemoveUser_RemovesUser_FromUserCollection()
    {
        var user = new User("id", "name", "email");
        var sut = new Organisation("id", "name") { Users = [user] };
        sut.RemoveUser(user);
        sut.Users.Should().BeEmpty();
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_ReturnsGenericObject_WithInstanceValues()
    {
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var organisation = new Organisation("id", "name");

        var expected = new { organisation.Id, organisation.Name, Sync = organisation.DateLastSynchronised.UtcDateTime };
        var actual = organisation.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
}