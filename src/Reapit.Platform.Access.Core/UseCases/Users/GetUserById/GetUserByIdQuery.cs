using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.GetUserById;

/// <summary>Request to retrieve details of an individual user.</summary>
/// <param name="Id">The unique identifier of the user.</param>
public record GetUserByIdQuery(string Id) : IRequest<User>;