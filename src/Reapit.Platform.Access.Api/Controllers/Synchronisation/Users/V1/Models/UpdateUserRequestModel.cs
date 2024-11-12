using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;

/// <summary>Represents the payload of a request to update a user.</summary>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
public record UpdateUserRequestModel(
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("email")] string Email);