using System.Text.Json.Serialization;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;

/// <summary>Representation of an organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
/// <param name="Name">The name of the organisation.</param>
/// <param name="DateLastSynchronised">The date and time on which the user was last synchronised.</param>
public record SimpleOrganisationModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("sync")] DateTime DateLastSynchronised);