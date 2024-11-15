using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;

/// <summary>Request model used when patching a role.</summary>
/// <param name="Name">The name of the role.</param>
/// <param name="Description">A description of the role.</param>
public record PatchRoleRequestModel(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description);