using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;

/// <summary>Request model used when fetching a collection of roles.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="UserId">Limit results to roles assigned to the user with this unique identifier.</param>
/// <param name="Name">Limit results to roles matching this name.</param>
/// <param name="Description">Limit results to roles matching this name.</param>
/// <param name="CreatedFrom">Limit results to roles created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to roles created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to roles last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to roles last modified before this date.</param>
public record GetRolesRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("userId")] string? UserId = null,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);