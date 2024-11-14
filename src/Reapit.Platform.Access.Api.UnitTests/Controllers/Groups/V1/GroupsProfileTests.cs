using Reapit.Platform.Access.Api.Controllers.Groups.V1;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Groups.V1;

public class GroupsProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<GroupsProfile>())
        .CreateMapper();

    private static readonly DateTime BaseDate = new(2020, 1, 1, 12, 30, 15, DateTimeKind.Utc); 
    
    /*
     * Group => GroupModel
     */

    [Fact]
    public void GroupsProfile_PopulatesGroupModel_FromGroupEntity()
    {
        const string name = "group-name", organisationId = "organisation-id";
        var entity = GetEntity(name, string.Empty, organisationId);
        
        var expected = new GroupModel(entity.Id, entity.Name, entity.OrganisationId, entity.DateCreated, entity.DateModified);
        var actual = _mapper.Map<GroupModel>(entity);
        actual.Should().BeEquivalentTo(expected);
        
        // DateTimes should know that they're UTC so they get suffixed appropriately
        actual.DateCreated.Kind.Should().Be(DateTimeKind.Utc);
        actual.DateModified.Kind.Should().Be(DateTimeKind.Utc);

    }
    
    /*
     * GetGroupsRequestModel => GetGroupsQuery
     */

    [Fact]
    public void GroupsProfile_PopulatesGetGroupsQuery_FromGetGroupsRequestModel()
    {
        var request = new GetGroupsRequestModel(
            1234,
            72,
            "user-id",
            "organisation-id",
            "name",
            "description",
            BaseDate.AddHours(1),
            BaseDate.AddHours(2),
            BaseDate.AddHours(3),
            BaseDate.AddHours(4)
        );

        var expected = new GetGroupsQuery(
            request.Cursor, 
            request.PageSize, 
            request.UserId, 
            request.OrganisationId,
            request.Name, 
            request.Description, 
            request.CreatedFrom, 
            request.CreatedTo, 
            request.ModifiedFrom,
            request.ModifiedTo);

        var actual = _mapper.Map<GetGroupsQuery>(request);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * IEnumerable<Group> => PagedResult<GroupModel>
     */

    [Fact]
    public void GroupsProfile_PopulatesPagedResult_FromGroupsCollection()
    {
        const int expectedPageSize = 5;
        var groups = Enumerable.Range(0, expectedPageSize)
            .Select(i => GetEntity($"group-{i}", $"group-{i}-description", $"group-{i}-organisation", i))
            .ToList();

        var expectedCursor = groups.Max(group => group.Cursor);
        var expectedContent = groups.Select(group => _mapper.Map<GroupModel>(group));

        var actual = _mapper.Map<ResultPage<GroupModel>>(groups);
        actual.Data.Should().BeEquivalentTo(expectedContent);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(expectedPageSize);
    }
    
    /*
     * Private methods
     */

    private static Group GetEntity(
        string name,
        string description,
        string organisationId,
        int dateOffset = 0)
    {
        var guid = Guid.NewGuid();
        using var fixedGuidProvider = new GuidProviderContext(guid);
        
        var fixedTime = new DateTimeOffset(BaseDate, TimeSpan.Zero).AddDays(dateOffset);
        using var fixedTimeProvider = new DateTimeOffsetProviderContext(fixedTime);

        return new Group(name, description, organisationId)
        {
            DateModified = fixedTime.AddHours(10).UtcDateTime
        };
    }
}