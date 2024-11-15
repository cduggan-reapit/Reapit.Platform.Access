using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;

/// <summary>Request for a single roles.</summary>
/// <param name="Id">The unique identifier of the role.</param>
public record GetRoleByIdQuery(string Id) : IRequest<Role>;