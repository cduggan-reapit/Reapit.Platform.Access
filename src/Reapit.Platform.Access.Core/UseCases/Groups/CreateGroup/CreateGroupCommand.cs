using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;

/// <summary>Command to create a group.</summary>
/// <param name="Name">The name of the group.</param>
/// <param name="Description">A description of the group.</param>
/// <param name="OrganisationId">The unique identifier of the organisation to which the group belongs.</param>
public record CreateGroupCommand(
    string Name, 
    string? Description, 
    string OrganisationId) 
    : IRequest<Group>;