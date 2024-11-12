using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.UnitTests.TestHelpers;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Data.UnitTests.Repositories.Organisations;

public class OrganisationRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetOrganisationByIdAsync
     */

    [Fact]
    public async Task GetOrganisationByIdAsync_ReturnsNull_WhenOrganisationNotFound()
    {
        const string id = "organisation-999";

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetOrganisationByIdAsync(id, default);
        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task GetOrganisationByIdAsync_ReturnsOrganisationWithMatchingId_WhenOrganisationExists()
    {
        const string id = "organisation-100";

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetOrganisationByIdAsync(id, default);
        actual.Should().NotBeNull()
            .And.Match(organisation => (organisation as Organisation)!.Id == id);
    }

    /*
     * CreateOrganisationAsync
     */
    
    [Fact]
    public async Task CreateOrganisationAsync_AddsEntityToChangeTracker()
    {
        var organisation = new Organisation("test-id", "test-name");
        await using var context = await GetContextAsync();
        var sut = CreateSut(context);
        _ = await sut.CreateOrganisationAsync(organisation, default);

        context.ChangeTracker.Entries<Organisation>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * UpdateOrganisationAsync
     */
    
    [Fact]
    public async Task UpdateOrganisationAsync_MarksEntityAsModified_InChangeTracker()
    {
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var organisation = await context.Organisations.SingleAsync(o => o.Id == "organisation-137");
        organisation.Update("new name");
        
        var sut = CreateSut(context);
        _ = await sut.UpdateOrganisationAsync(organisation, default);

        context.ChangeTracker.Entries<Organisation>()
            .Where(entry => entry.State == EntityState.Modified)
            .Should().HaveCount(1);
    }

    /*
     * DeleteOrganisationAsync
     */
    
    [Fact]
    public async Task DeleteOrganisationAsync_MarksEntityAsDeleted_InChangeTracker()
    {
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var organisation = await context.Organisations.SingleAsync(o => o.Id == "organisation-137");
        
        var sut = CreateSut(context);
        _ = await sut.DeleteOrganisationAsync(organisation, default);

        context.ChangeTracker.Entries<Organisation>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
 
    /*
     * AddMemberAsync
     */

    [Fact]
    public async Task AddMemberAsync_AddsOrganisationUser_ToChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var user = await context.Users.SingleAsync(u => u.Id == "user-001-2");
        var organisation = await context.Organisations.SingleAsync(o => o.Id == "organisation-002");

        var member = new OrganisationUser(organisation, user);

        var sut = CreateSut(context);
        await sut.AddMemberAsync(member, default);

        context.ChangeTracker.Entries<OrganisationUser>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * RemoveMemberAsync
     */
    
    [Fact]
    public async Task RemoveMemberAsync_MarksMemberAsDeleted_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var organisation = await context.Organisations
            .Include(o => o.OrganisationUsers)
            .SingleAsync(o => o.Id == "organisation-002");
        
        var member = organisation.OrganisationUsers.Single(ou => ou.UserId == "user-002-2");

        var sut = CreateSut(context);
        await sut.RemoveMemberAsync(member, default);

        context.ChangeTracker.Entries<OrganisationUser>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static OrganisationRepository CreateSut(AccessDbContext context)
        => new(context);

    private static readonly DateTime BaseDateTime = new(2020, 1, 1, 0, 00, 0); 
    
    private static async Task PlantSeedDataAsync(AccessDbContext context)
    {
        // 200 organisations, each with 3 users
        var organisations = Enumerable.Range(1, 200)
            .Select(i =>
            {
                var time = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(i - 1);
                using var timeFixture = new DateTimeOffsetProviderContext(time);

                var organisationId = $"organisation-{i:D3}";
                
                return new Organisation(organisationId, $"Organisation {i:D3}")
                {
                    OrganisationUsers = Enumerable.Range(1, 3).Select(u =>
                    {
                        // Offset the user times so they don't conflict with one another
                        var userTime = time.AddHours(u - 1);
                        using var userTimeFixture = new DateTimeOffsetProviderContext(userTime);
                        
                        var userId = $"user-{i:D3}-{u}";
                        return new OrganisationUser
                        {
                            OrganisationId = organisationId,
                            UserId = userId,
                            User = new User(userId, $"User {i:D3}-{u}", "test@example.net")
                        };
                    }).ToList()
                };
            });

        await context.Organisations.AddRangeAsync(organisations);
        await context.SaveChangesAsync();
    }
}