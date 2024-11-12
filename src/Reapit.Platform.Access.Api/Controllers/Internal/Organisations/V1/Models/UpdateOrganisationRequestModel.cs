namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;

/// <summary>Request model used when updating an organisation.</summary>
/// <param name="Name">The name of the organisation.</param>
public record UpdateOrganisationRequestModel(string Name);