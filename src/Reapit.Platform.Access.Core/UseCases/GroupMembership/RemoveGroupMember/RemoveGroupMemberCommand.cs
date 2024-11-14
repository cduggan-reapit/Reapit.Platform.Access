using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.GroupMembership.RemoveGroupMember;

/// <summary>Command to remove a user from a group.</summary>
/// <param name="GroupId">The unique identifier of the group.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record RemoveGroupMemberCommand(string GroupId, string UserId) : IRequest;