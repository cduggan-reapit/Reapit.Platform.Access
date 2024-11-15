using System.Net;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.IntegrationTests.Controllers.Internal.V1;

public class UsersControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    
    /*
     * PUT /api/internal/users/{id}
     */

    [Fact]
    public async Task SynchroniseUser_ReturnsNoContent_WhenUserCreated()
    {
        await InitializeDatabaseAsync();
        const string id = "user-000";
        var requestModel = new SynchroniseUserRequestModel("new name", "new email");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/users/{id}", content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task SynchroniseUser_ReturnsNoContent_WhenUserUpdates()
    {
        await InitializeDatabaseAsync();
        const string id = "user-007";
        var requestModel = new SynchroniseUserRequestModel("new name", "new email");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/users/{id}", content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task SynchroniseUser_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/users/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task SynchroniseUser_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/users/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task SynchroniseUser_ReturnsUnprocessable_WhenRequestInvalid()
    {
        const string id = "user-000";
        var requestModel = new SynchroniseUserRequestModel(new string('a', 1000), "new email");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/users/{id}", content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    /*
     * DELETE /api/internal/users/{id}
     */

    [Fact]
    public async Task DeleteUserById_ReturnsNoContent_WhenUserDeleted()
    {
        const string id = "user-006", url = $"/api/internal/users/{id}";
        await InitializeDatabaseAsync();
        
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUserById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/users/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteUserById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/users/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/users/user-000");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
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

        await dbContext.Users.AddRangeAsync(SeedUsers);

        _ = await dbContext.SaveChangesAsync();
    }

    private static IEnumerable<User> SeedUsers 
        => Enumerable.Range(1, 10)
            .Select(i =>
            {
                using var _ = new DateTimeOffsetProviderContext(BaseDate.AddDays(i - 1));
                return new User($"user-{i:D3}", $"User {i:D3}", $"user-{i:D3}@test.net");
            })
            .ToList();
}