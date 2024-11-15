using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;

/// <summary>Lightweight representation of a role.</summary>
/// <param name="Id">The unique identifier of the role.</param>
/// <param name="Name">The name of the role.</param>
/// <param name="DateCreated">The date and time on which the role was created (UTC).</param>
/// <param name="DateModified">The date and time on which the role was last modified (UTC).</param>
public record RoleModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);