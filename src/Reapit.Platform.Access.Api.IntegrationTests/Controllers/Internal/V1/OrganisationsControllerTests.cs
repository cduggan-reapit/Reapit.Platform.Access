﻿using System.Net;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Reapit.Platform.Access.Data.Context;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Api.IntegrationTests.Controllers.Internal.V1;

public class OrganisationsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");

    /*
     * PUT /api/internal/organisations/{id}
     */

    [Fact]
    public async Task SynchroniseOrganisation_ReturnsNoContent_WhenOrganisationUpdates()
    {
        await InitializeDatabaseAsync();
        const string id = "organisation-007";
        var requestModel = new SynchroniseOrganisationRequestModel("new name");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task SynchroniseOrganisation_ReturnsNotFound_WhenOrganisationCreated()
    {
        await InitializeDatabaseAsync();
        const string id = "organisation-000";
        var requestModel = new SynchroniseOrganisationRequestModel("new name");
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task SynchroniseOrganisation_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/organisations/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task SynchroniseOrganisation_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Put, "/api/internal/organisations/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task SynchroniseOrganisation_ReturnsUnprocessable_WhenRequestInvalid()
    {
        const string id = "organisation-000";
        var requestModel = new SynchroniseOrganisationRequestModel(new string('a', 1000));
        var response = await SendRequestAsync(HttpMethod.Put, $"/api/internal/organisations/{id}", content: requestModel);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
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
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrganisationById_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteOrganisationById_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteOrganisationById_ReturnsNotFound_WhenOrganisationDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/organisation-000");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
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
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations/any/members/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Post, "/api/internal/organisations/any/members/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenOrganisationNotFound()
    {
        const string id = "organisation-999", 
            userId = "user-001-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddMember_ReturnsNotFound_WhenUserNotFound()
    {
        const string id = "organisation-007", 
            userId = "user-001-999", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task AddMember_ReturnsConflict_WhenRelationshipAlreadyExists()
    {
        const string id = "organisation-008", 
            userId = "user-008-001", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, url, content: null);
        await response.Should().HaveStatusCode(HttpStatusCode.Conflict).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceConflict);
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
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsBadRequest_WhenApiVersionNotProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any/members/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsBadRequest_WhenEndpointNotAvailableInVersion()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, "/api/internal/organisations/any/members/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsNotFound_WhenOrganisationNotFound()
    {
        const string id = "organisation-999", 
            userId = "user-001-003", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url, content: null);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteMember_ReturnsNotFound_WhenRelationshipNotFound()
    {
        const string id = "organisation-007", 
            userId = "user-001-002", 
            url = $"/api/internal/organisations/{id}/members/{userId}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url, content: null);
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
                    Users = Enumerable.Range(1, 3)
                        .Select(j => 
                            new User($"user-{i:D3}-{j:D3}", 
                                $"Organisation {i:D3} - User {j:D3}", 
                                $"user-{j:D3}@organisation-{i:D3}.net"))
                        .ToList()
                };
            })
            .ToList();
}