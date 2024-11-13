using FluentAssertions;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Domain.UnitTests.Entities;

public class GroupTests
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

        var sut = new Group(name, description, organisationId);

        // Explicit
        sut.Name.Should().Be(name);
        sut.Description.Should().Be(description);
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
        var sut = new Group("name", "description", "organisationId");
        sut.Update(null, null);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }
    
    [Fact]
    public void Update_DoesNotUpdateEntity_WhenValuesUnchanged()
    {
        var sut = new Group("name", "description", "organisationId");
        sut.Update(sut.Name, sut.Description);

        sut.IsDirty.Should().BeFalse();
        sut.DateModified.Should().Be(sut.DateCreated);
    }
    
    [Fact]
    public void Update_UpdatesEntity_WhenValuesChanged()
    {
        var sut = new Group("name", "description", "organisationId");
        sut.Update("new name", sut.Description);

        sut.IsDirty.Should().BeTrue();
        sut.DateModified.Should().NotBe(sut.DateCreated);
    }
}