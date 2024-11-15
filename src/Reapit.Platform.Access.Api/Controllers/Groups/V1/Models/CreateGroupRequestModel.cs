using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;

/// <summary>Request model used when creating a new group.</summary>
/// <param name="Name">The name of the group.</param>
/// <param name="Description">A description of the group.</param>
/// <param name="OrganisationId">The organisation to which the group belongs.</param>
public record CreateGroupRequestModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description, 
    [property: JsonPropertyName("organisationId")] string OrganisationId);