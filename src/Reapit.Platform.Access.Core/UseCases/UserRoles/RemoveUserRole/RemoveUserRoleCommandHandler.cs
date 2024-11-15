using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.UserRoles.RemoveUserRole;

/// <summary>Handler for the <see cref="RemoveUserRoleCommand"/> request.</summary>
public class RemoveUserRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveUserRoleCommandHandler> logger)
    : IRequestHandler<RemoveUserRoleCommand>
{
    /// <inheritdoc />
    public async Task Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        // Get Role (there'll be fewer roles than users so this _should_ be marginally cheaper)
        var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken)
                    ?? throw new NotFoundException(typeof(Role), request.RoleId);
        
        // If the user's not in the role, throw not found
        var user = role.Users.FirstOrDefault(user => user.Id == request.UserId);
        if(user == null)
            throw new NotFoundException("Membership", request.ToJson());
        
        // Update from the role side - that's the domain this service controls.
        role.RemoveUser(user);
        _ = await unitOfWork.Roles.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {userId} removed from role {roleId}", request.UserId, request.RoleId);
    }
}