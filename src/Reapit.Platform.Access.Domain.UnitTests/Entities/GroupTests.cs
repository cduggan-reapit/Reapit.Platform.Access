﻿using FluentAssertions;
using System.Text.Json;
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
    
    /*
     * AddUser
     */

    [Fact]
    public void AddUser_AddsUserToCollection()
    {
        var user = new User("user-id", "user-name", "user-email");
        var sut = new Group("name", "description", "organisationId");
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
        var sut = new Group("name", "description", "organisationId")
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
        var sut = new Group( "name", "description", "organisation-id");
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
    public void AsSerializable_ReturnsAnonymousObject_ForGroup()
    {
        const string name = "name", description = "description", organisationId = "organisation-id";
        
        var group = new Group(name, description, organisationId);
        var expected = new { group.Id, group.Name, group.Description, group.OrganisationId, group.DateCreated, group.DateModified };
        
        var actual = group.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_ForEntity()
    {
        var entity = new Group("name", "description", "organisationId");
        var expected = JsonSerializer.Serialize(entity.AsSerializable());
        var actual = entity.ToString();
        actual.Should().Be(expected);
    }
}