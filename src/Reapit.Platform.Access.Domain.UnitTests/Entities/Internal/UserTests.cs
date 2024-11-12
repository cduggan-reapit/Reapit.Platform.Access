using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities.Internal;

public class UserTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesEntity_FromParameters()
    {
        var timeFixture = new DateTimeOffset(2016, 4, 5, 9, 5, 16, TimeSpan.FromHours(-6));
        using var _ = new DateTimeOffsetProviderContext(timeFixture);
        const string id = "user-id", name = "name", email = "test@example.net";

        var sut = new User(id, name, email);
        sut.Id.Should().Be(id);
        sut.Name.Should().Be(name);
        sut.Email.Should().Be(email);
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
        const string id = "user-id", name = "name", email = "test@example.net";

        var sut = new User(id, "initial", "initial");
        sut.DateLastSynchronised.Should().Be(firstTimeFixture);
        
        using var secondContext = new DateTimeOffsetProviderContext(secondTimeFixture);
        sut.Update(name, email);
        sut.Id.Should().Be(id);
        sut.Name.Should().Be(name);
        sut.Email.Should().Be(email);
        sut.DateLastSynchronised.Should().Be(secondTimeFixture);
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_ReturnsGenericObject_WithInstanceValues()
    {
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var user = new User("id", "name", "email");

        var expected = new { user.Id, user.Name, user.Email, Sync = user.DateLastSynchronised.UtcDateTime };
        var actual = user.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
}