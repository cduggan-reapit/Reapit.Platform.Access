using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;

/// <summary>Command used to update a group.</summary>
/// <param name="Id">The unique identifier of the group.</param>
/// <param name="Name">The name of the group.</param>
/// <param name="Description">A description of the group.</param>
public record PatchGroupCommand(string Id, string? Name, string? Description) 
    : IRequest<Group>;