using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;

/// <summary>Represents a command to add a user to an organisation.</summary>
/// <param name="OrganisationId">The unique identifier of the organisation.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record AddOrganisationMemberCommand(string OrganisationId, string UserId) : IRequest;