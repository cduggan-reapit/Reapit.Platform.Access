using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;

/// <summary>Lightweight representation of a group.</summary>
/// <param name="Id">The unique identifier of the group.</param>
/// <param name="Name">The name of the group.</param>
/// <param name="OrganisationId">The unique identifier of the organisation to which the group belongs.</param>
/// <param name="DateCreated">The date and time on which the group was created (UTC).</param>
/// <param name="DateModified">The date and time on which the group was last modified (UTC).</param>
public record GroupModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("organisationId")] string OrganisationId, 
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);