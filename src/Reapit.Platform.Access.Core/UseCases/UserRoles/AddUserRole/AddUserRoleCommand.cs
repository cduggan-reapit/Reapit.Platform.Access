using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.UserRoles.AddUserRole;

/// <summary>Command to add a user to a role.</summary>
/// <param name="RoleId">The unique identifier of the role.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record AddUserRoleCommand(string RoleId, string UserId) : IRequest;