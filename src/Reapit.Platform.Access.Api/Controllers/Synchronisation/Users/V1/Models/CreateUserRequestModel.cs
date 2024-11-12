using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;

/// <summary>Represents the payload of a user event notification.</summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
public record CreateUserRequestModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("email")] string Email);