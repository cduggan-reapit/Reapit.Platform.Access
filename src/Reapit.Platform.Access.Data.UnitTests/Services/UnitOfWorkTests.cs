using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Data.UnitTests.Services;

public class UnitOfWorkTests : DatabaseAwareTestBase
{
    /*
     * Users
     */
    
    [Fact]
    public async Task Users_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.Users;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Users_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.Users;
        var subsequent = sut.Users;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * Organisations
     */
    
    [Fact]
    public async Task Organisations_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.Organisations;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Organisations_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.Organisations;
        var subsequent = sut.Organisations;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * Groups
     */
    
    [Fact]
    public async Task Groups_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.Groups;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Groups_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.Groups;
        var subsequent = sut.Groups;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * SaveChangesAsync
     */

    [Fact]
    public async Task SaveChangesAsync_CommitsChangesToDatabase_WhenCalledAfterChangesMadeInRepository()
    {
        // var dummy = new Dummy("dummy name");
        
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        
        // CreateAsync should add one - check that it's state is Added
        await sut.Users.CreateUserAsync(new User("id", "name", "email"), default);
        dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Added));
        
        await sut.SaveChangesAsync(default);

        // Once it's saved, it should be committed and thus tracked as Unchanged
        dbContext.Users.Should().HaveCount(1);
        dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Unchanged));
    }
    
    [Fact]
    public async Task SaveChangesAsync_DoesNotThrow_WhenNoChangesTracked()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        
        await sut.SaveChangesAsync(default);
        dbContext.Users.Should().HaveCount(0);
    }
    
    /*
     * Private methods
     */

    private static UnitOfWork CreateSut(AccessDbContext context)
        => new(context);
}