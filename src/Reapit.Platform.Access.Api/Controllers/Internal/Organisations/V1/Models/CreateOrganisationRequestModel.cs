namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;

/// <summary>Request model used when creating a new organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
/// <param name="Name">The name of the organisation.</param>
public record CreateOrganisationRequestModel(string Id, string Name);