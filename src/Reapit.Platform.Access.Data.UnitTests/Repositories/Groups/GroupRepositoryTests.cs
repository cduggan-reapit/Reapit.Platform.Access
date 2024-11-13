using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Access.Domain.Entities.Transient;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Data.UnitTests.Repositories.Groups;

public class GroupRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetGroupsAsync
     */

    [Fact]
    public async Task GetGroupsAsync_ReturnsPagedResult_WithDefaultValues()
    {
        const int pageSize = 25;
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var expectedGroupIds = context.Groups.OrderBy(o => o.Cursor)
            .Take(pageSize)
            .Select(item => item.Id)
            .ToList();
        
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync();
        actual.Should().HaveCount(pageSize)
            .And.AllSatisfy(group => expectedGroupIds.Should().Contain(group.Id));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsPagedResult_WhenCursorProvided()
    {
        const int pageSize = 5;
        const int pageNumber = 4;
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        // Calculate the cursor:
        // The "cursor" is exclusive, using a cursor > value statement.  We therefore need to get the maximum cursor that
        // would have been returned in page 3.  With applied default page size of 5 and zero-based indexing, this is
        // item 14 in the seed data population. 
        var cursorDateTime = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(14);
        var cursor = (long)(cursorDateTime - DateTimeOffset.UnixEpoch).TotalMicroseconds;

        var expectedGroupIds = context.Groups.OrderBy(o => o.Cursor)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(item => item.Id)
            .ToList();
        
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(pageSize: pageSize, cursor: cursor);
        actual.Should().HaveCount(pageSize)
            .And.AllSatisfy(group => expectedGroupIds.Should().Contain(group.Id));
    }

    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenUserIdProvided()
    {
        const string userId = "user-006";

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(userId: userId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(group => group.GroupUsers.Should().Contain(groupUser => groupUser.OrganisationUser.UserId == userId));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenOrganisationIdProvided()
    {
        const string organisationId = "organisation-02";

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(organisationId: organisationId);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(group => group.OrganisationId.Should().Be(organisationId));
    }

    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenNameProvided()
    {
        const string name = "Group 08";
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(name: name);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(group => group.Name.Should().Be(name));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenDescriptionProvided()
    {
        const string description = "Description of Group 1";
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(description: description);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(group => group.Description.Should().StartWith(description));
    }

    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenCreatedFromProvided()
    {
        var createdFrom = BaseDateTime.AddDays(15);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(createdFrom: createdFrom);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(group => group.DateCreated.Should().BeOnOrAfter(createdFrom));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenCreatedToProvided()
    {
        var createdTo = BaseDateTime.AddDays(15);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(createdTo: createdTo);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(group => group.DateCreated.Should().BeBefore(createdTo));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenModifiedFromProvided()
    {
        var modifiedFrom = BaseDateTime.AddDays(15).AddYears(1);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(modifiedFrom: modifiedFrom);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(group => group.DateModified.Should().BeOnOrAfter(modifiedFrom));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenModifiedToProvided()
    {
        var modifiedTo = BaseDateTime.AddDays(15).AddYears(1);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(modifiedTo: modifiedTo);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(group => group.DateModified.Should().BeBefore(modifiedTo));
    }

    /*
     * GetByIdAsync
     */

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenRecordNotFound()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync("does not exist", default);
        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenRecordFound()
    {
        const int groupIndex = 27;
        var id = new Guid($"{groupIndex:D32}").ToString("N");
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().NotBeNull()
            .And.Match((Group group) => group.Id == id);
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracker()
    {
        var entity = new Group("group-name", "group-description", "organisation-01");
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        _ = await sut.CreateAsync(entity, default);

        context.ChangeTracker.Entries<Group>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * UpdateAsync
     */
    
    [Fact]
    public async Task UpdateAsync_SetsEntityAsModified_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var entity = context.Groups.Skip(7).First();
        entity.Update("new name", null);
        
        var sut = CreateSut(context);
        _ = await sut.UpdateAsync(entity, default);

        context.ChangeTracker.Entries<Group>()
            .Where(entry => entry.State == EntityState.Modified)
            .Should().HaveCount(1);
    }
    
    /*
     * DeleteAsync
     */
    
    [Fact]
    public async Task DeleteAsync_SetsEntityAsDeleted_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var entity = context.Groups.Skip(12).First();
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<Group>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static readonly DateTime BaseDateTime = new DateTime(2020, 1, 1, 0, 0, 0);
    
    private static GroupRepository CreateSut(AccessDbContext context)
        => new(context);
    
    private static async Task PlantSeedDataAsync(AccessDbContext context)
    {
        var users = Enumerable.Range(1, 60)
            .Select(u => (Index: u, Entity: new User($"user-{u:D3}", $"User {u:D3}", $"user-{u:D3}@test.net")))
            .ToDictionary(item => item.Index, item => item.Entity);
        
        var organisations = Enumerable.Range(1, 2)
            .Select(o => (Index: o, Entity: new Organisation($"organisation-{o:D2}", $"Organisation {o:D2}")))
            .ToDictionary(item => item.Index, item => item.Entity);
        
        var organisationUsers = Enumerable.Range(1, 2)
            .SelectMany(o =>
            {
                var organisationId = $"organisation-{o:D2}";
                var userIds = users.Where(u => u.Key % 2 + 1 == o)
                    .Select(u => u.Value.Id)
                    .ToArray();

                return userIds.Select(userId => new OrganisationUser
                {
                    OrganisationId = organisationId, 
                    UserId = userId 
                });
            });
        
        await context.Organisations.AddRangeAsync(organisations.Values);
        await context.Users.AddRangeAsync(users.Values);
        await context.OrganisationUsers.AddRangeAsync(organisationUsers);
        
        // Commit!
        await context.SaveChangesAsync();
        
        /*
         * Now make some groups!
         */

        var groups = Enumerable.Range(1, 30)
            .Select(index =>
            {
                var time = BaseDateTime.AddDays(index - 1);
                var guid = new Guid($"{index:D32}");
                var organisationId = $"organisation-{index % 2 + 1:D2}";

                using var timeFixture = new DateTimeOffsetProviderContext(time);
                using var guidFixture = new GuidProviderContext(guid);
                
                // We'll infer the userIds from the index:
                var firstOrganisationUser = context.OrganisationUsers.Single(ou => ou.UserId == $"user-{index:D3}");
                var secondOrganisationUser = context.OrganisationUsers.Single(ou => ou.UserId == $"user-{index + 30:D3}");
                
                return new Group($"Group {index:D2}", $"Description of Group {index:D2}", organisationId)
                {
                    DateModified = time.AddYears(1),
                    GroupUsers = [
                        new GroupUser { GroupId = $"{guid:N}", OrganisationUser = firstOrganisationUser },
                        new GroupUser { GroupId = $"{guid:N}", OrganisationUser = secondOrganisationUser }
                    ]
                };
            });
        
        await context.Groups.AddRangeAsync(groups);
        await context.SaveChangesAsync();

    }
}