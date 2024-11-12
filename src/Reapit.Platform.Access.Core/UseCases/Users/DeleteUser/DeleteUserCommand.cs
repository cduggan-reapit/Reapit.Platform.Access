using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;

/// <summary>Command to delete a user.</summary>
/// <param name="Id">The unique identifier of the user.</param>
public record DeleteUserCommand(string Id) : IRequest<User>;