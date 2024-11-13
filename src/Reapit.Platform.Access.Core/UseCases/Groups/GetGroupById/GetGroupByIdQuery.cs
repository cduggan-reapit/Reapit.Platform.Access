using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;

public record GetGroupByIdQuery(string Id) : IRequest<Group>;