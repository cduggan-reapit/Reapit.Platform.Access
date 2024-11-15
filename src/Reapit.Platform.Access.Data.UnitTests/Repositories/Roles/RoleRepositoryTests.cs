using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Data.UnitTests.Repositories.Roles;

public class RoleRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetRolesAsync
     */

    [Fact]
    public async Task GetRolesAsync_ReturnsUnfilteredPage_WhenNoParametersProvided()
    {
        // Get the default page size with no offset
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync();
        actual.Should().HaveCount(25);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsUnfilteredPage_WhenPaginationDetailsProvided()
    {
        // Get the default page size with no offset
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        // Skip 5 items, take 5 items
        var expected = context.Roles.OrderBy(r => r.Cursor).Skip(5).Take(5);
        var cursor = await expected.MinAsync(role => role.Cursor) - 1;
        var ids = await expected.Select(role => role.Id).ToListAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(pagination: new PaginationFilter(cursor, 5));
        actual.Should().HaveCount(5)
            .And.AllSatisfy(role => ids.Should().Contain(role.Id));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenUserIdProvided()
    {
        const string userId = "user-067";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(userId: userId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(role => role.Users.Any(user => user.Id == userId).Should().BeTrue());
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenNameProvided()
    {
        const string name = "Role 068";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(name: name);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(role => role.Name.Should().Be(name));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenDescriptionProvided()
    {
        const string description = "Role 06";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(description: description);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(role => role.Description.Should().Contain(description));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenCreatedFromProvided()
    {
        // There are 200 roles (with created dates from BaseDate +0 to BaseDate +199 days).
        // The createdFrom date is INCLUSIVE, so if we try to get +185 to +199 there should be 15 results:
        // [185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199]
        var createdFrom = BaseDateTime.AddDays(185);
        var dateFilter = new TimestampFilter(CreatedFrom: createdFrom);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(15)
            .And.AllSatisfy(role => role.DateCreated.Should().BeOnOrAfter(createdFrom));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenCreatedToProvided()
    {
        // There are 200 roles (with created dates from BaseDate +0 to BaseDate +199 days).
        // The createdTo date is EXCLUSIVE, so if we try to get +0 to +10 there should be 10 results:
        // [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        var createdTo = BaseDateTime.AddDays(10);
        var dateFilter = new TimestampFilter(CreatedTo: createdTo);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(role => role.DateCreated.Should().BeBefore(createdTo));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenModifiedFromProvided()
    {
        // There are 200 roles (with modified dates from BaseDate +0 to BaseDate +199 days).
        // The modifiedFrom date is INCLUSIVE, so if we try to get +195 to +199 there should be 5 results:
        // [195, 196, 197, 198, 199]
        var modifiedFrom = BaseDateTime.AddDays(195).AddYears(1);
        var dateFilter = new TimestampFilter(ModifiedFrom: modifiedFrom);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(role => role.DateModified.Should().BeOnOrAfter(modifiedFrom));
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsFilteredPage_WhenModifiedToProvided()
    {
        // There are 200 roles (with modified dates from BaseDate +0 to BaseDate +199 days).
        // The modifiedTo date is EXCLUSIVE, so if we try to get +0 to +5 there should be 5 results:
        // [0, 1, 2, 3, 4]
        var modifiedTo = BaseDateTime.AddDays(5).AddYears(1);
        var dateFilter = new TimestampFilter(ModifiedTo: modifiedTo);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetRolesAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(role => role.DateCreated.Should().BeBefore(modifiedTo));
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
        const int roleIndex = 27;
        var id = new Guid($"{roleIndex:D32}").ToString("N");
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().NotBeNull()
            .And.Match((Role role) => role.Id == id);
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracker()
    {
        var entity = new Role("role-name", "role-description");
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        _ = await sut.CreateAsync(entity, default);

        context.ChangeTracker.Entries<Role>()
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
        
        var entity = context.Roles.Skip(7).First();
        entity.Update("new name", null);
        
        var sut = CreateSut(context);
        _ = await sut.UpdateAsync(entity, default);

        context.ChangeTracker.Entries<Role>()
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
        
        var entity = context.Roles.Skip(12).First();
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<Role>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static readonly DateTime BaseDateTime = new(2020, 1, 1, 0, 0, 0);
    
    private static RoleRepository CreateSut(AccessDbContext context)
        => new(context);
    
    private static async Task PlantSeedDataAsync(AccessDbContext context)
    {
        var users = Enumerable.Range(0, 400)
            .Select(u => (Index: u, Entity: new User($"user-{u:D3}", $"User {u:D3}", $"user-{u:D3}@test.net")))
            .ToDictionary(item => item.Index, item => item.Entity);

        var roles = Enumerable.Range(0, 200)
            .Select(seed =>
            {
                var guidFixture = new Guid($"{seed:D32}");
                using var guidContext = new GuidProviderContext(guidFixture);
                
                var timeFixture = DateTime.SpecifyKind(BaseDateTime.AddDays(seed), DateTimeKind.Utc);
                using var timeContext = new DateTimeOffsetProviderContext(timeFixture);
                
                var role = new Role($"Role {seed:D3}", $"Description of Role {seed:D3}")
                {
                    DateModified = timeFixture.AddYears(1),
                    Users = [ users[seed], users[seed+200] ]
                };

                return role;
            })
            .ToList();

        await context.Users.AddRangeAsync(users.Values);
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}