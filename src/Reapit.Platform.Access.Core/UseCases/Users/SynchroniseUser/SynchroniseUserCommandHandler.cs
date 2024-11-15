using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.SynchroniseUser;

/// <summary>Handler for the <see cref="SynchroniseUserCommand"/> request.</summary>
public class SynchroniseUserCommandHandler(
    IUnitOfWork unitOfWork, 
    IValidator<SynchroniseUserCommand> validator, 
    ILogger<SynchroniseUserCommandHandler> logger) 
    : IRequestHandler<SynchroniseUserCommand, User>
{
    /// <inheritdoc />
    public async Task<User> Handle(SynchroniseUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var user = await unitOfWork.Users.GetUserByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            user = new User(request.Id, request.Name, request.Email);
            await unitOfWork.Users.CreateUserAsync(user, cancellationToken);
        }
        else
        {
            user.Update(request.Name, request.Email);
            await unitOfWork.Users.UpdateUserAsync(user, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("User synchronised: {id} ({blob})", request.Id, user.AsSerializable());

        return user;
    }
}