namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Organisations.V1.Models;

/// <summary>Representation of an organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
/// <param name="Name">The name of the organisation.</param>
public record SimpleOrganisationModel(string Id, string Name);