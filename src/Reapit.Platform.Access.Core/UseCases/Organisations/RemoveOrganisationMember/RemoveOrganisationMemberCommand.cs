using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;

/// <summary>Represents a command to remove a user from an organisation.</summary>
/// <param name="OrganisationId">The unique identifier of the organisation.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record RemoveOrganisationMemberCommand(string OrganisationId, string UserId) : IRequest;