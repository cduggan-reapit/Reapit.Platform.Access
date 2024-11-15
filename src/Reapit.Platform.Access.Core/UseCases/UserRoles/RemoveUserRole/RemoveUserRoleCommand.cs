using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.UserRoles.RemoveUserRole;

/// <summary>Command to remove a user from a role.</summary>
/// <param name="RoleId">The unique identifier of the role.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record RemoveUserRoleCommand(string RoleId, string UserId) : IRequest;