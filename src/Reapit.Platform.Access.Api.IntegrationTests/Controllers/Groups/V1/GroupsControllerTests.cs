using System.Net;
using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Groups.V1;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.IntegrationTests.Controllers.Groups.V1;

public class GroupsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<GroupsProfile>())
        .CreateMapper();
    
    /*
     * GET /api/groups
     */

    [Fact]
    public async Task GetGroups_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetGroups_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetGroups_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetGroups_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<GroupModel>>(SeedGroups.Take(3));
        
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET /api/groups/{id}
     */
    
    [Fact]
    public async Task GetGroupById_ReturnsOk_WithGroupModel()
    {
        const int seed = 7;
        var id = Guid.Parse($"{seed:D32}").ToString("N");
        await InitializeDatabaseAsync();
        
        var group = SeedGroups.Single(u => u.Id == id);
        var expected = _mapper.Map<GroupModel>(group);

        var response = await SendRequestAsync(HttpMethod.Get, $"/api/groups/{id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }

    [Fact]
    public async Task GetGroupById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetGroupById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetGroupById_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, "/api/groups/any");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync();
    }
    
    /*
     * POST /api/groups/
     */

    [Fact]
    public async Task CreateGroup_ReturnsCreated_WhenGroupCreated()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateGroupRequestModel("test-group", "description of test group", "organisation-01");
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups", content: requestModel);
        
        await response.Should().HaveStatusCode(HttpStatusCode.Created)
            .And.MatchPayloadAsync<GroupModel>(actual => 
                actual.Name == requestModel.Name && 
                actual.OrganisationId == requestModel.OrganisationId);
    }
    
    [Fact]
    public async Task CreateGroup_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task CreateGroup_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateGroup_ReturnsUnprocessable_WhenRequestInvalid()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateGroupRequestModel("group-011", new string('a', 1001), "organisation-01");
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups", content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed); 
    }
    
    /*
     * PATCH /api/groups/{id}
     */
    
    [Fact]
    public async Task PatchGroup_ReturnsNoContent_WhenGroupPatched()
    {
        const int id = 7;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchGroupRequestModel("new name", null);
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        var checkContent = await check.Content.ReadFromJsonAsync<GroupModel>();
        checkContent!.Name.Should().BeEquivalentTo("new name");
    }
    
    [Fact]
    public async Task PatchGroup_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, "/api/groups/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task PatchGroup_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, "/api/groups/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchGroup_ReturnsUnprocessable_WhenRequestInvalid()
    {
        const int id = 6;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchGroupRequestModel("valid-name", new string('a', 1001));
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed); 
    }
    
    [Fact]
    public async Task PatchGroup_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        const int id = 11;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchGroupRequestModel("valid name", "valid description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound); 
    }
    
    /*
     * DELETE /api/groups/{id}
     */
    
    [Fact]
    public async Task DeleteGroup_ReturnsNoContent_WhenGroupDeleted()
    {
        const int id = 7;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteGroup_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/groups/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteGroup_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/groups/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteGroup_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        const int id = 11;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound); 
    }
    
    /*
     * POST /api/groups/{id}/members/{userId}
     */
    
    [Fact]
    public async Task AddMember_ReturnsNoContent_WhenAddedToGroup()
    {
        await InitializeDatabaseAsync();
        
        // The zero user is in organisation-01 but not associated with any groups
        var user = SeedUsers[2];
        
        // Group 6 is a member of organisation-01
        const int id = 6;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}/members/{user.Id}";
        
        var response = await SendRequestAsync(HttpMethod.Post, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups/any/members/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/groups/any/members/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenUserNotAssociatedWithGroupOrganisation()
    {
        await InitializeDatabaseAsync();
        
        // The zero user is in organisation-01 but not associated with any groups
        var user = SeedUsers[2];
        
        // Group 7 is a member of organisation-02
        const int id = 7;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}/members/{user.Id}";
        
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.InvalidGroup);
    }
    
    [Fact]
    public async Task AddMember_ReturnsConflict_WhenUserAlreadyAssociatedWithGroup()
    {
        await InitializeDatabaseAsync();
        
        // The zero user is in organisation-01 but not associated with any groups
        var user = SeedUsers[2];
        
        // Group 6 is a member of organisation-01
        const int id = 6;
        var guid = $"{id:D32}";
        var url = $"/api/groups/{guid}/members/{user.Id}";
        
        // It's fiddly to seed these, just run it twice - the second one should fail.
        var setup = await SendRequestAsync(HttpMethod.Post, url);
        setup.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.Conflict).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceConflict);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenGroupNotExists()
    {
        await InitializeDatabaseAsync();
        const string url = "/api/groups/missing/members/user-00";
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenUserNotExists()
    {
        await InitializeDatabaseAsync();
        const int id = 6;
        var url = $"/api/groups/{id:D32}/members/user-99";
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
      
    /*
    /// <summary>Remove a user from a group.</summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpDelete("{id}/members/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> RemoveMember([FromRoute] string id, [FromRoute] string userId)
    {
        var request = new RemoveGroupMemberCommand(GroupId: id, UserId: userId);
        await mediator.Send(request);
        return NoContent();
    }*/
    
    /*
     * Private methods
     */

    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<AccessDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        // Adding the users will add the organisations
        var organisations = SeedOrganisations;
        var users = SeedUsers;
        
        organisations[0].AddUser(users[0]);
        organisations[0].AddUser(users[2]);
        organisations[1].AddUser(users[1]);
        
        await dbContext.Organisations.AddRangeAsync(organisations);
        await dbContext.Users.AddRangeAsync(users);
        _ = await dbContext.SaveChangesAsync();
        
        // Configure user-organisation relationships:
        var groups = SeedGroups;
        for(var g = 0; g < groups.Count; g++)
            groups[g].AddUser(users[(g + 1) % 2]);
        
        await dbContext.Groups.AddRangeAsync(SeedGroups);
        
        _ = await dbContext.SaveChangesAsync();
    }

    private static IList<Organisation> SeedOrganisations =>
    [
        new Organisation("organisation-01", "First Organisation"),
        new Organisation("organisation-02", "Second Organisation")
    ];

    private static IList<User> SeedUsers =>
    [
        new User("user-01", "First User", "first@example.net"),
        new User("user-02", "Second User", "second@example.net"),
        new User("user-00", "Not Associated User", "zero@example.net")
    ];

    private static IList<Group> SeedGroups 
        => Enumerable.Range(1, 10)
            .Select(GetSeedGroup)
            .ToList();

    private static Group GetSeedGroup(int seed)
    {
        using var timeProvider = new DateTimeOffsetProviderContext(BaseDate.AddDays(seed - 1));
        using var guidProvider = new GuidProviderContext(new Guid($"{seed:D32}"));

        var organisationIndex = seed % 2;
        var group = new Group($"Group {seed:D2}", $"Description of Group {seed:D2}", $"organisation-{seed:D2}")
        {
            OrganisationId = $"organisation-{organisationIndex + 1:D2}"
        };

        return group;
    }
}