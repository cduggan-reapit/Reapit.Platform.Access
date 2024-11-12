using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;

/// <summary>Command to update a user.</summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
public record UpdateUserCommand(string Id, string Name, string Email) : IRequest<User>;