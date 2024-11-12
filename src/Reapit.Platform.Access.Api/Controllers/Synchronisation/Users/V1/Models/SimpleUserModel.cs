using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;

/// <summary>Simple representation of a user.</summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The users name.</param>
/// <param name="Email">The users email address.</param>
/// <param name="DateLastSynchronised">The date and time on which the user was last synchronised.</param>
public record SimpleUserModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("email")] string Email, 
    [property: JsonPropertyName("sync")] DateTime DateLastSynchronised);