using FluentAssertions;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Data.UnitTests.Repositories.Groups;

public class GroupFilterHelperTests
{
    /*
     * ApplyCursor
     */

    [Fact]
    public void ApplyCursor_DoesNotApplyFilter_WhenCursorIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCursorFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCursor_AppliesFilter_WhenCursorProvided()
    {
        // There should be 60 records with a cursor greater than this:
        var cursorTime = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(29);
        var cursor = (long)(cursorTime - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        var data = SeedData;
        var actual = data.ApplyCursorFilter(cursor);
        actual.Should().HaveCount(60);
    }
    
    /*
     * ApplyUserIdFilter
     */
    
    [Fact]
    public void ApplyUserIdFilter_DoesNotApplyFilter_WhenUserIdIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyUserIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyUserIdFilter_AppliesFilter_WhenUserIdProvided()
    {
        // User 004 is associated with 30 records
        const string userId = "user-004";
        var data = SeedData;
        var actual = data.ApplyUserIdFilter(userId);
        actual.Should().HaveCount(30);
    }
    
    /*
     * ApplyOrganisationIdFilter
     */
    
    [Fact]
    public void ApplyOrganisationIdFilter_DoesNotApplyFilter_WhenOrganisationIdIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyOrganisationIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyOrganisationIdFilter_AppliesFilter_WhenOrganisationIdProvided()
    {
        const string organisationId = "organisation-002";
        var data = SeedData;
        var actual = data.ApplyOrganisationIdFilter(organisationId);
        actual.Should().HaveCount(30);
    }
    
    /*
     * ApplyNameFilter
     */
    
    [Fact]
    public void ApplyNameFilter_DoesNotApplyFilter_WhenNameIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyNameFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyNameFilter_AppliesFilter_WhenNameProvided()
    {
        const string name = "Group 042";
        var data = SeedData;
        var actual = data.ApplyNameFilter(name);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplyDescriptionFilter
     */
    
    [Fact]
    public void ApplyDescriptionFilter_DoesNotApplyFilter_WhenDescriptionIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyDescriptionFilter_AppliesFilter_WhenDescriptionProvided()
    {
        const string description = "042";
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(description);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplyCreatedFromFilter
     */
    
    [Fact]
    public void ApplyCreatedFromFilter_DoesNotApplyFilter_WhenCreatedFromIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCreatedFromFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCreatedFromFilter_AppliesFilter_WhenCreatedFromProvided()
    {
        var createdFrom = BaseDateTime.AddDays(59);
        
        var data = SeedData;
        var actual = data.ApplyCreatedFromFilter(createdFrom);
        actual.Should().HaveCount(31, "CreatedTo is inclusive");
    }
    
    /*
     * ApplyCreatedToFilter
     */
    
    [Fact]
    public void ApplyCreatedToFilter_DoesNotApplyFilter_WhenCreatedToIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCreatedToFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCreatedToFilter_AppliesFilter_WhenCreatedToProvided()
    {
        var createdTo = BaseDateTime.AddDays(59);
        
        var data = SeedData;
        var actual = data.ApplyCreatedToFilter(createdTo);
        actual.Should().HaveCount(59, "CreatedTo is exclusive");
    }
    
    /*
     * ApplyModifiedFromFilter
     */
    
    [Fact]
    public void ApplyModifiedFromFilter_DoesNotApplyFilter_WhenModifiedFromIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyModifiedFromFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyModifiedFromFilter_AppliesFilter_WhenModifiedFromProvided()
    {
        var modifiedFrom = BaseDateTime.AddDays(60).AddYears(1);
        
        var data = SeedData;
        var actual = data.ApplyModifiedFromFilter(modifiedFrom);
        actual.Should().HaveCount(30, "ModifiedTo is inclusive");
    }
    
    /*
     * ApplyModifiedToFilter
     */
    
    [Fact]
    public void ApplyModifiedToFilter_DoesNotApplyFilter_WhenModifiedToIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyModifiedToFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyModifiedToFilter_AppliesFilter_WhenModifiedToProvided()
    {
        var modifiedTo = BaseDateTime.AddDays(60).AddYears(1);
        
        var data = SeedData;
        var actual = data.ApplyModifiedToFilter(modifiedTo);
        actual.Should().HaveCount(60, "ModifiedTo is exclusive");
    }
    
    /*
     * Private methods
     */

    private static readonly DateTime BaseDateTime = new DateTime(2020, 1, 1, 0, 0, 0);

    private static IQueryable<Group> SeedData 
        => Enumerable.Range(1, 90).Select(GetSeedGroup).AsQueryable();
    
    private static Group GetSeedGroup(int seedValue)
    {
        var guid = new Guid($"{seedValue:D32}");
        var time = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(seedValue - 1);
        
        using var guidFixture = new GuidProviderContext(guid);
        using var timeFixture = new DateTimeOffsetProviderContext(time);

        // Spread groups across three organisations (organisation-001, organisation-002, organisation-003).
        // Each with two users:
        // 001 => 001 & 002
        // 002 => 002 & 003
        // 003 => 003 & 004
        var organisationSeed = seedValue % 3 + 1;
        return new Group($"Group {seedValue:D3}", $"Group {seedValue:D3} Description", $"organisation-{organisationSeed:D3}")
        {
            DateModified = time.UtcDateTime.AddYears(1),
            Users = Enumerable.Range(0, 2)
                .Select(u => new User($"user-{organisationSeed + u:D3}", "name", "email"))
                .ToList()
        };
    }
}