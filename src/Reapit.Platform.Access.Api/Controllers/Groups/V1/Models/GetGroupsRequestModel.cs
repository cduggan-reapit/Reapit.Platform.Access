using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;

/// <summary>Request model used when fetching a collection of groups.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="UserId">Limit results to groups associated with the user with this unique identifier.</param>
/// <param name="OrganisationId">Limit results to groups associated with the organisation with this unique identifier.</param>
/// <param name="Name">Limit results to groups matching this name.</param>
/// <param name="Description">Limit results to groups matching this name.</param>
/// <param name="CreatedFrom">Limit results to groups created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to groups created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to groups last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to groups last modified before this date.</param>
public record GetGroupsRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("userId")] string? UserId = null,
    [property: JsonPropertyName("organisationId")] string? OrganisationId = null,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);