namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;

/// <summary>Request model used when creating a new group.</summary>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="OrganisationId"></param>
public record CreateUserGroupRequestModel(string Name, string Description, string OrganisationId);