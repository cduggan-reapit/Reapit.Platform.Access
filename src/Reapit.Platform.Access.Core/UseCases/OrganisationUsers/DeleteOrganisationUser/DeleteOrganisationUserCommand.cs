using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.OrganisationUsers.DeleteOrganisationUser;

/// <summary>Represents a command to remove a user from an organisation.</summary>
/// <param name="OrganisationId">The unique identifier of the organisation.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record DeleteOrganisationUserCommand(string OrganisationId, string UserId) : IRequest<OrganisationUser>;