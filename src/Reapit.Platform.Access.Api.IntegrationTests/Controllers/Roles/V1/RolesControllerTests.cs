using System.Net;
using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Roles.V1;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.IntegrationTests.Controllers.Roles.V1;

public class RolesControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<RolesProfile>())
        .CreateMapper();
    
    /*
     * GET /api/roles
     */

    [Fact]
    public async Task GetRoles_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetRoles_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetRoles_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetRoles_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<RoleModel>>(SeedRoles.Take(3));
        
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET /api/roles/{id}
     */
    
    [Fact]
    public async Task GetRoleById_ReturnsOk_WithRoleModel()
    {
        const int seed = 7;
        var id = Guid.Parse($"{seed:D32}").ToString("N");
        await InitializeDatabaseAsync();
        
        var role = SeedRoles.Single(u => u.Id == id);
        var expected = _mapper.Map<RoleModel>(role);

        var response = await SendRequestAsync(HttpMethod.Get, $"/api/roles/{id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }

    [Fact]
    public async Task GetRoleById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetRoleById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetRoleById_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, "/api/roles/any");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync();
    }
    
    /*
     * POST /api/roles/
     */

    [Fact]
    public async Task CreateRole_ReturnsCreated_WhenRoleCreated()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateRoleRequestModel("test-role", "description of test role");
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles", content: requestModel);
        
        await response.Should().HaveStatusCode(HttpStatusCode.Created)
            .And.MatchPayloadAsync<RoleModel>(actual => 
                actual.Name == requestModel.Name);
    }
    
    [Fact]
    public async Task CreateRole_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task CreateRole_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateRole_ReturnsUnprocessable_WhenRequestInvalid()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateRoleRequestModel("role-011", new string('a', 1001));
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles", content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed); 
    }
    
    /*
     * PATCH /api/roles/{id}
     */
    
    [Fact]
    public async Task PatchRole_ReturnsNoContent_WhenRolePatched()
    {
        const int id = 7;
        var url = $"/api/roles/{id:D32}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchRoleRequestModel("new name", null);
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        var checkContent = await check.Content.ReadFromJsonAsync<RoleModel>();
        checkContent!.Name.Should().BeEquivalentTo("new name");
    }
    
    [Fact]
    public async Task PatchRole_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, "/api/roles/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task PatchRole_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, "/api/roles/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchRole_ReturnsUnprocessable_WhenRequestInvalid()
    {
        const int id = 6;
        var url = $"/api/roles/{id:D32}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchRoleRequestModel("valid-name", new string('a', 1001));
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed); 
    }
    
    [Fact]
    public async Task PatchRole_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        const int id = 11;
        var url = $"/api/roles/{id:D32}";
        
        await InitializeDatabaseAsync();
        var requestModel = new PatchRoleRequestModel("valid name", "valid description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound); 
    }
    
    /*
     * DELETE /api/roles/{id}
     */
    
    [Fact]
    public async Task DeleteRole_ReturnsNoContent_WhenRoleDeleted()
    {
        const int id = 7;
        var url = $"/api/roles/{id:D32}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteRole_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/roles/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteRole_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/roles/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        const int id = 11;
        var url = $"/api/roles/{id:D32}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound); 
    }
    
    /*
     * POST /api/roles/{id}/users/{userId}
     */
    
    [Fact]
    public async Task AddUser_ReturnsNoContent_WhenAddedToRole()
    {
        await InitializeDatabaseAsync();
        
        var user = SeedUsers[0];
        
        const int id = 6;
        var url = $"/api/roles/{id:D32}/users/{user.Id}";
        
        var response = await SendRequestAsync(HttpMethod.Post, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles/any/users/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/roles/any/users/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task AddUser_ReturnsConflict_WhenUserAlreadyAssociatedWithRole()
    {
        await InitializeDatabaseAsync();
        
        var user = SeedUsers[1];
        
        const int id = 6;
        var url = $"/api/roles/{id:D32}/users/{user.Id}";
        
        var setup = await SendRequestAsync(HttpMethod.Post, url);
        setup.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.Conflict)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceConflict);
    }
    
    [Fact]
    public async Task AddUser_ReturnsNotFound_WhenRoleNotExists()
    {
        await InitializeDatabaseAsync();
        const string url = "/api/roles/missing/users/user-00";
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddUser_ReturnsNotFound_WhenUserNotExists()
    {
        await InitializeDatabaseAsync();
        const int id = 6;
        var url = $"/api/roles/{id:D32}/users/user-99";
        var response = await SendRequestAsync(HttpMethod.Post, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    /*
     * DELETE /api/roles/{id}/users/{user-id}
     */
    
    [Fact]
    public async Task RemoveUser_ReturnsNoContent_WhenRemovedFromRole()
    {
        await InitializeDatabaseAsync();
        
        var user = SeedUsers[0];
        
        const int id = 6;
        var url = $"/api/roles/{id:D32}/users/{user.Id}";

        // Add the user to the role (confirm it worked)
        var setup = await SendRequestAsync(HttpMethod.Post, url);
        setup.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        // Remove the user from the role (this is the test!)
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task RemoveUser_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/roles/any/users/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task RemoveUser_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/roles/any/users/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task RemoveUser_ReturnsNotFound_WhenRoleNotExists()
    {
        await InitializeDatabaseAsync();
        const string url = "/api/roles/missing/users/user-00";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task RemoveUser_ReturnsNotFound_WhenUserNotAssignedRole()
    {
        await InitializeDatabaseAsync();
        const int id = 6;
        var url = $"/api/roles/{id:D32}/users/user-99";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
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

        
        await dbContext.Users.AddRangeAsync(SeedUsers);
        await dbContext.Roles.AddRangeAsync(SeedRoles);
        _ = await dbContext.SaveChangesAsync();
    }

    private static IList<User> SeedUsers =>
    [
        new User("user-01", "First User", "first@example.net"),
        new User("user-02", "Second User", "second@example.net")
    ];

    private static IList<Role> SeedRoles 
        => Enumerable.Range(1, 10)
            .Select(GetSeedRole)
            .ToList();

    private static Role GetSeedRole(int seed)
    {
        using var timeProvider = new DateTimeOffsetProviderContext(BaseDate.AddDays(seed - 1));
        using var guidProvider = new GuidProviderContext(new Guid($"{seed:D32}"));

        var role = new Role($"Role {seed:D2}", $"Description of Role {seed:D2}");

        return role;
    }
}