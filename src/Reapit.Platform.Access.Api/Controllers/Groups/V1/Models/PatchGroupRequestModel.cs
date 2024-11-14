using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;

/// <summary>Request model used when updating a group.</summary>
/// <param name="Name">The name of the group.</param>
/// <param name="Description">A description of the group.</param>
public record PatchGroupRequestModel(
    [property: JsonPropertyName("name")] string? Name, 
    [property: JsonPropertyName("description")] string? Description);