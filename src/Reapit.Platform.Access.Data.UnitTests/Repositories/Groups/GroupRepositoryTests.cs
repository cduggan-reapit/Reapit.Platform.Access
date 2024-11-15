using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;
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
            .And.AllSatisfy(group => group.Users.Should().Contain(groupUser => groupUser.Id == userId));
    }
    
    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenOrganisationIdProvided()
    {
        // We've got four groups per organisation
        const string organisationId = "organisation-02";
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(organisationId: organisationId);
        actual.Should().HaveCount(4)
            .And.AllSatisfy(group => group.OrganisationId.Should().Be(organisationId));
    }

    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenNameProvided()
    {
        const string name = "Group 087";
        
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
        const string description = "Group 01";
        // The filter is "contains" so if we filter for Group 01, we expect 10 results (010 - 019, inclusive)
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(description: description);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(group => group.Description.Should().Contain(description));
    }

    [Fact]
    public async Task GetGroupsAsync_ReturnsFilteredResult_WhenCreatedFromProvided()
    {
        // Get the last 10 (skip 0-190, return 191-200)
        var createdFrom = BaseDateTime.AddDays(190);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetGroupsAsync(createdFrom: createdFrom);
        actual.Should().HaveCount(10)
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
        // Get the last 15 (skip 0-185, return 186-200)
        var modifiedFrom = BaseDateTime.AddDays(185).AddYears(1);
        
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
        var actual = await sut.GetGroupByIdAsync("does not exist", default);
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
        var actual = await sut.GetGroupByIdAsync(id, default);
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
        /*
         * For the sake of the repository, we don't actually care which organisation users belong to.  The requirement
         * for a user to be associated with the same organisation that their groups are is enforced by validation rather
         * than by structure.
         */
        
        var users = Enumerable.Range(0, 400)
            .Select(u => (Index: u, Entity: new User($"user-{u:D3}", $"User {u:D3}", $"user-{u:D3}@test.net")))
            .ToDictionary(item => item.Index, item => item.Entity);
        
        var organisations = Enumerable.Range(0, 50)
            .Select(o => (Index: o, Entity: new Organisation($"organisation-{o:D2}", $"Organisation {o:D2}")))
            .ToDictionary(item => item.Index, item => item.Entity);

        var groups = Enumerable.Range(0, 200)
            .Select(seed =>
            {
                var guidFixture = new Guid($"{seed:D32}");
                using var guidContext = new GuidProviderContext(guidFixture);
                
                var timeFixture = DateTime.SpecifyKind(BaseDateTime.AddDays(seed), DateTimeKind.Utc);
                using var timeContext = new DateTimeOffsetProviderContext(timeFixture);
                
                var organisation = organisations[seed % 50];
                var group = new Group($"Group {seed:D3}", $"Description of Group {seed:D3}", organisation.Id)
                {
                    DateModified = timeFixture.AddYears(1),
                    Organisation = organisation,
                    Users = [ users[seed], users[seed+200] ]
                };

                return group;
            })
            .ToList();

        await context.Organisations.AddRangeAsync(organisations.Values);
        await context.Users.AddRangeAsync(users.Values);
        await context.Groups.AddRangeAsync(groups);
        await context.SaveChangesAsync();
    }
}