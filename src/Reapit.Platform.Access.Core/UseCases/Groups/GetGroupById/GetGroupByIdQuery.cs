using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;

/// <summary>Request to get an individual group.</summary>
/// <param name="Id">The unique identifier of the group.</param>
public record GetGroupByIdQuery(string Id) : IRequest<Group>;