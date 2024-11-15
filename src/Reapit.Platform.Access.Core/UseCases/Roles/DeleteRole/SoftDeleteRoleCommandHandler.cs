using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.DeleteRole;

/// <summary>Handler for the <see cref="SoftDeleteRoleCommand"/> request.</summary>
public class SoftDeleteRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<SoftDeleteRoleCommandHandler> logger) 
    : IRequestHandler<SoftDeleteRoleCommand, Role>
{
    /// <inheritdoc />
    public async Task<Role> Handle(SoftDeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Roles.GetRoleByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException(typeof(Group), request.Id);
        
        role.SoftDelete();
        _ = await unitOfWork.Roles.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Role soft-deleted: {id} ({blob})", request.Id, role.AsSerializable().ToJson());

        return role;
    }
}