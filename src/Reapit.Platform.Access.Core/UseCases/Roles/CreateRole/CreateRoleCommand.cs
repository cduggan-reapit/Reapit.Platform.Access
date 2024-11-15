using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;

/// <summary>Command to create a role.</summary>
/// <param name="Name">The name of the role.</param>
/// <param name="Description">A description of the role.</param>
public record CreateRoleCommand(string Name, string? Description) : IRequest<Role>;