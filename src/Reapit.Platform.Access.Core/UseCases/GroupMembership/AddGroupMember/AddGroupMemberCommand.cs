using MediatR;

namespace Reapit.Platform.Access.Core.UseCases.GroupMembership.AddGroupMember;

/// <summary>Command to add a user to a group.</summary>
/// <param name="GroupId">The unique identifier of the group.</param>
/// <param name="UserId">The unique identifier of the user.</param>
public record AddGroupMemberCommand(string GroupId, string UserId) : IRequest;