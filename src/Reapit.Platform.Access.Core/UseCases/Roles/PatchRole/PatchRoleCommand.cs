using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;

/// <summary>Command used to update a role.</summary>
/// <param name="Id">The unique identifier of the role.</param>
/// <param name="Name">The name of the role.</param>
/// <param name="Description">A description of the role.</param>
public record PatchRoleCommand(string Id, string? Name, string? Description) 
    : IRequest<Role>;