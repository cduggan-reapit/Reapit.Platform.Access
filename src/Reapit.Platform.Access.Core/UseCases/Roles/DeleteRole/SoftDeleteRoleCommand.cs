using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.DeleteRole;

/// <summary>Command used to soft-delete a role.</summary>
/// <param name="Id">The unique identifier of the role.</param>
public record SoftDeleteRoleCommand(string Id) : IRequest<Role>;