using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.DeleteGroup;

/// <summary>Command used to soft-delete a group.</summary>
/// <param name="Id">The unique identifier of the group.</param>
public record SoftDeleteGroupCommand(string Id) : IRequest<Group>;