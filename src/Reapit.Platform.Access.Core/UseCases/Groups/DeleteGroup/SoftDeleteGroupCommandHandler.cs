using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.DeleteGroup;

/// <summary>Handler for the <see cref="SoftDeleteGroupCommand"/> request.</summary>
public class SoftDeleteGroupCommandHandler(IUnitOfWork unitOfWork, ILogger<SoftDeleteGroupCommandHandler> logger) 
    : IRequestHandler<SoftDeleteGroupCommand, Group>
{
    /// <inheritdoc />
    public async Task<Group> Handle(SoftDeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await unitOfWork.Groups.GetGroupByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException(typeof(Group), request.Id);
        
        group.SoftDelete();
        _ = await unitOfWork.Groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Group soft-deleted: {id} ({blob})", request.Id, group.AsSerializable().ToJson());
        
        return group;
    }
}