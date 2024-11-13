using AutoMapper;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.IntegrationTests.Controllers.Internal;

public class OrganisationsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<OrganisationsProfile>())
        .CreateMapper();
    
    /*
     * GET /api/internal/organisations/{id}
     */

    [Fact]
    public async Task GetOrganisationById_ReturnsOk_WithOrganisationModel()
    {
        const string id = "organisation-006";
        await InitializeDatabaseAsync();
        
        var organisation = SeedOrganisations.Single(u => u.Id == id);
        var expected = _mapper.Map<SimpleOrganisationModel>(organisation);

        var response = await SendRequestAsync(HttpMethod.Get, $"/api/internal/organisations/{id}");
        await response.Should().HaveStatusCode(200).And.HavePayloadAsync(expected);
    }

    [Fact]
    public async Task GetOrganisationById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/internal/organisations/any", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetOrganisationById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Get, "/api/internal/organisations/any", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync();
    }
    
    [Fact]
    public async Task GetOrganisationById_ReturnsNotFound_WhenOrganisationDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, "/api/internal/organisations/organisation-000");
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync();
    }
    
    /*
     * POST /api/internal/organisations
     */

    [Fact]
    public async Task CreateOrganisation_ReturnsCreated_WhenOrganisationCreated()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateOrganisationRequestModel("organisation-011", "Organisation Eleven");
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations", content: requestModel);
        
        await response.Should().HaveStatusCode(201)
            .And.MatchPayloadAsync<SimpleOrganisationModel>(actual => 
                actual.Id == requestModel.Id && 
                actual.Name == requestModel.Name);
    }
    
    [Fact]
    public async Task CreateOrganisation_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task CreateOrganisation_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateOrganisation_ReturnsConflict_WhenOrganisationAlreadyExists()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateOrganisationRequestModel("organisation-001", "Organisation One");
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations", content: requestModel);
        await response.Should().HaveStatusCode(409).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceConflict); 
    }
    
    [Fact]
    public async Task CreateOrganisation_ReturnsUnprocessable_WhenRequestInvalid()
    {
        await InitializeDatabaseAsync();
        var requestModel = new CreateOrganisationRequestModel("organisation-011", new string('a', 1000));
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations", content: requestModel);
        await response.Should().HaveStatusCode(422).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed); 
    }
    
    /*
     * PUT /api/internal/organisations/{id}
     */

    [Fact]
    public async Task UpdateOrganisation_ReturnsNoContent_WhenOrganisationUpdates()
    {
        await InitializeDatabaseAsync();
        const string id = "organisation-007";
        var requestModel = new UpdateOrganisationRequestModel("new name");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        response.Should().HaveStatusCode(204);
    }
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/organisations/any", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/organisations/any", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsNotFound_WhenOrganisationDoesNotExist()
    {
        await InitializeDatabaseAsync();
        const string id = "organisation-000";
        var requestModel = new UpdateOrganisationRequestModel("new name");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task UpdateOrganisation_ReturnsUnprocessable_WhenRequestInvalid()
    {
        const string id = "organisation-000";
        var requestModel = new UpdateOrganisationRequestModel(new string('a', 1000));
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        await response.Should().HaveStatusCode(422).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    /*
     * GET /api/internal/organisations/{id}
     */

    [Fact]
    public async Task DeleteOrganisationById_ReturnsNoContent_WhenOrganisationDeleted()
    {
        const string id = "organisation-006", url = $"/api/internal/organisations/{id}";
        await InitializeDatabaseAsync();
        
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(204);

        // Following deletion, GET should return a 404
        var deleteCheck = await SendRequestAsync(HttpMethod.Get, url);
        deleteCheck.Should().HaveStatusCode(404);
    }

    [Fact]
    public async Task DeleteOrganisationById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteOrganisationById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteOrganisationById_ReturnsNotFound_WhenOrganisationDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/organisation-000");
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    /*
     * POST /api/internal/organisations/{id}/members/{userId}
     */

    [Fact]
    public async Task AddMember_ReturnsNoContent_WhenUserAdded()
    {
        const string id = "organisation-006", 
            userId = "user-001-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        response.Should().HaveStatusCode(204);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations/any/members/any", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations/any/members/any", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenOrganisationNotFound()
    {
        const string id = "organisation-999", 
            userId = "user-001-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenUserNotFound()
    {
        const string id = "organisation-007", 
            userId = "user-001-999", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddMember_ReturnsConflict_WhenRelationshipAlreadyExists()
    {
        const string id = "organisation-008", 
            userId = "user-008-001", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(409).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceConflict);
    }

    /*
     * DELETE /api/internal/organisations/{id}/members/{userId}
     */

    [Fact]
    public async Task DeleteMember_ReturnsNoContent_WhenUserDeleted()
    {
        const string id = "organisation-006", 
            userId = "user-006-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url, content: null);
        response.Should().HaveStatusCode(204);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any/members/any", null);
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any/members/any", "0.9");
        await response.Should().HaveStatusCode(400).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsNotFound_WhenOrganisationNotFound()
    {
        const string id = "organisation-999", 
            userId = "user-001-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url, content: null);
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsNotFound_WhenRelationshipNotFound()
    {
        const string id = "organisation-007", 
            userId = "user-001-002", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url, content: null);
        await response.Should().HaveStatusCode(404).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    /*
     * Private methods
     */

    private async Task InitializeDatabaseAsync()
    {
        TestApiFactory.TimeFixture = new DateTimeOffset(2022, 12, 1, 12, 1, 12, TimeSpan.FromHours(-6));
        
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<AccessDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Organisations.AddRangeAsync(SeedOrganisations);

        _ = await dbContext.SaveChangesAsync();
    }

    private static IEnumerable<Organisation> SeedOrganisations 
        => Enumerable.Range(1, 10)
            .Select(i =>
            {
                using var _ = new DateTimeOffsetProviderContext(BaseDate.AddDays(i - 1));
                var organisationId = $"organisation-{i:D3}";
                return new Organisation(organisationId, $"Organisation {i:D3}")
                {
                    OrganisationUsers = Enumerable.Range(1, 3)
                        .Select(j => new OrganisationUser
                        {
                            OrganisationId = organisationId,
                            User = new User(
                                $"user-{i:D3}-{j:D3}", 
                                $"Organisation {i:D3} - User {j:D3}", 
                                $"user-{j:D3}@organisation-{i:D3}.net")
                        })
                        .ToList()
                };
            })
            .ToList();
}