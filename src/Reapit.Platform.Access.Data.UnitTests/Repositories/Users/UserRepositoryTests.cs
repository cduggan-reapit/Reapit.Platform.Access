using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Data.UnitTests.Repositories.Users;

public class UserRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetUserByIdAsync
     */

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        const string id = "user-99";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);

        var actual = await sut.GetUserByIdAsync(id, default);
        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserNotFound()
    {
        const string id = "user-37";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);

        var actual = await sut.GetUserByIdAsync(id, default);
        actual.Should().NotBeNull()
            .And.Match(user => (user as User)!.Id == id);
    }
    
    /*
     * CreateUserAsync
     */

    [Fact]
    public async Task CreateUserAsync_AddsEntityToChangeTracker()
    {
        var user = new User("test-id", "test-name", "test-email");
        await using var context = await GetContextAsync();
        var sut = CreateSut(context);
        _ = await sut.CreateUserAsync(user, default);

        context.ChangeTracker.Entries<User>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * UpdateUserAsync
     */
    
    [Fact]
    public async Task UpdateUserAsync_FlagsEntityAsModified_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var user = await context.Users.SingleAsync(item => item.Id == "user-39");
        user.Update("updated-name", "updated-email");
        
        var sut = CreateSut(context);
        _ = await sut.UpdateUserAsync(user, default);

        context.ChangeTracker.Entries<User>()
            .Where(entry => entry.State == EntityState.Modified)
            .Should().HaveCount(1);
    }
    
    /*
     * DeleteUserAsync
     */
    
    [Fact]
    public async Task DeleteUserAsync_FlagsEntityAsDeleted_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var user = await context.Users.SingleAsync(item => item.Id == "user-17");
        
        var sut = CreateSut(context);
        _ = await sut.DeleteUserAsync(user, default);

        context.ChangeTracker.Entries<User>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }

    /*
     * Private methods
     */

    private static UserRepository CreateSut(AccessDbContext context)
        => new(context);

    private static readonly DateTime BaseDateTime = new(2020, 1, 1, 0, 00, 0); 
    
    private static async Task PlantSeedDataAsync(AccessDbContext context)
    {
        // We'll create 90 users across three organisations
        
        // Three organisations:
        var organisations = Enumerable.Range(1, 3)
            .Select(o =>
            {
                using var timeContext = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(o - 1));
                return (Index: o, Value: new Organisation($"organisation-{o}", $"Test Organisation {o}"));
            })
            .ToDictionary(tuple => tuple.Index, tuple => tuple.Value);
        
        // Ninety users:
        var users = Enumerable.Range(1, 90)
            .Select(u =>
            {
                var userId = $"user-{u:D2}";
                var time = BaseDateTime.AddDays(u - 1);
                using var timeContext = new DateTimeOffsetProviderContext(time);
                return new User(userId, $"User {u:D2}", $"user-{u:D2}@example.net")
                {
                    Organisations = [
                        organisations[u % 3 + 1]
                    ]
                };
            })
            .ToList();

        await context.Organisations.AddRangeAsync(organisations.Values);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}