using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.GroupMembership.RemoveGroupMember;

/// <summary>Handler for the <see cref="RemoveGroupMemberCommand"/> request.</summary>
public class RemoveGroupMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveGroupMemberCommandHandler> logger)
    : IRequestHandler<RemoveGroupMemberCommand>
{
    /// <inheritdoc />
    public async Task Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
    {
        // Get Group (there'll be fewer groups than users so this _should_ be marginally cheaper)
        var group = await unitOfWork.Groups.GetByIdAsync(request.GroupId, cancellationToken)
                    ?? throw new NotFoundException(typeof(Group), request.GroupId);
        
        // If the user's not in the group, throw not found
        var user = group.Users.FirstOrDefault(user => user.Id == request.UserId);
        if(user == null)
            throw new NotFoundException("Membership", request.ToJson());
        
        // Update from the group side - that's the domain this service controls.
        group.RemoveUser(user);
        _ = await unitOfWork.Groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {userId} removed from group {groupId}", request.UserId, request.GroupId);
    }
}