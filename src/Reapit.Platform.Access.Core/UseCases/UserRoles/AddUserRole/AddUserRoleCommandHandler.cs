using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Access.Core.UseCases.UserRoles.AddUserRole;

/// <summary>Handler for the <see cref="AddUserRoleCommand"/> request.</summary>
public class AddUserRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<AddUserRoleCommandHandler> logger)
    : IRequestHandler<AddUserRoleCommand>
{
    /// <inheritdoc />
    public async Task Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        // Get Role (there'll be fewer roles than users so this _should_ be marginally cheaper)
        var role = await unitOfWork.Roles.GetRoleByIdAsync(request.RoleId, cancellationToken)
                    ?? throw new NotFoundException(typeof(Role), request.RoleId);
        
        // If the user's already in the role, throw conflict
        if(role.Users.Any(user => user.Id == request.UserId))
            throw ConflictException.ResourceExists("Membership", request.ToJson());
        
        // Otherwise get the user
        var user = await unitOfWork.Users.GetUserByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(typeof(User), request.UserId);
        
        // Update from the role side - that's the domain this service controls.
        role.AddUser(user);
        _ = await unitOfWork.Roles.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {userId} added to role {roleId}", request.UserId, request.RoleId);
    }
}