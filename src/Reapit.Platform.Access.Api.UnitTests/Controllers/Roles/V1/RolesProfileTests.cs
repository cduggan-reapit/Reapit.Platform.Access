using Reapit.Platform.Access.Api.Controllers.Roles.V1;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Roles.V1;

public class RolesProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<RolesProfile>())
        .CreateMapper();

    private static readonly DateTime BaseDate = new(2020, 1, 1, 12, 30, 15, DateTimeKind.Utc); 
    
    /*
     * Role => RoleModel
     */

    [Fact]
    public void RolesProfile_PopulatesRoleModel_FromRoleEntity()
    {
        const string name = "role-name";
        var entity = GetEntity(name, string.Empty);
        
        var expected = new RoleModel(entity.Id, entity.Name, entity.DateCreated, entity.DateModified);
        var actual = _mapper.Map<RoleModel>(entity);
        actual.Should().BeEquivalentTo(expected);
        
        // DateTimes should know that they're UTC so they get suffixed appropriately
        actual.DateCreated.Kind.Should().Be(DateTimeKind.Utc);
        actual.DateModified.Kind.Should().Be(DateTimeKind.Utc);

    }
    
    /*
     * GetRolesRequestModel => GetRolesQuery
     */

    [Fact]
    public void RolesProfile_PopulatesGetRolesQuery_FromGetRolesRequestModel()
    {
        var request = new GetRolesRequestModel(
            1234,
            72,
            "user-id",
            "name",
            "description",
            BaseDate.AddHours(1),
            BaseDate.AddHours(2),
            BaseDate.AddHours(3),
            BaseDate.AddHours(4)
        );

        var expected = new GetRolesQuery(
            request.Cursor, 
            request.PageSize, 
            request.UserId, 
            request.Name, 
            request.Description, 
            request.CreatedFrom, 
            request.CreatedTo, 
            request.ModifiedFrom,
            request.ModifiedTo);

        var actual = _mapper.Map<GetRolesQuery>(request);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * IEnumerable<Role> => PagedResult<RoleModel>
     */

    [Fact]
    public void RolesProfile_PopulatesPagedResult_FromRolesCollection()
    {
        const int expectedPageSize = 5;
        var roles = Enumerable.Range(0, expectedPageSize)
            .Select(i => GetEntity($"role-{i}", $"role-{i}-description", i))
            .ToList();

        var expectedCursor = roles.Max(role => role.Cursor);
        var expectedContent = roles.Select(role => _mapper.Map<RoleModel>(role));

        var actual = _mapper.Map<ResultPage<RoleModel>>(roles);
        actual.Data.Should().BeEquivalentTo(expectedContent);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(expectedPageSize);
    }
    
    [Fact]
    public void RolesProfile_PopulatesPagedResult_FromEmptyCollection()
    {
        var input = Array.Empty<Role>();
        var expected = new ResultPage<RoleModel>(_mapper.Map<IEnumerable<RoleModel>>(input), 0, 0);

        var actual = _mapper.Map<ResultPage<RoleModel>>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private static Role GetEntity(
        string name,
        string description,
        int dateOffset = 0)
    {
        var guid = Guid.NewGuid();
        using var fixedGuidProvider = new GuidProviderContext(guid);
        
        var fixedTime = new DateTimeOffset(BaseDate, TimeSpan.Zero).AddDays(dateOffset);
        using var fixedTimeProvider = new DateTimeOffsetProviderContext(fixedTime);

        return new Role(name, description)
        {
            DateModified = fixedTime.AddHours(10).UtcDateTime
        };
    }
}