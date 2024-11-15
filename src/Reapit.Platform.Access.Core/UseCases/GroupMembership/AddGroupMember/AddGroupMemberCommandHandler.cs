using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Access.Core.UseCases.GroupMembership.AddGroupMember;

/// <summary>Handler for the <see cref="AddGroupMemberCommand"/> request.</summary>
public class AddGroupMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<AddGroupMemberCommandHandler> logger)
    : IRequestHandler<AddGroupMemberCommand>
{
    /// <inheritdoc />
    public async Task Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        // Get Group (there'll be fewer groups than users so this _should_ be marginally cheaper)
        var group = await unitOfWork.Groups.GetGroupByIdAsync(request.GroupId, cancellationToken)
                    ?? throw new NotFoundException(typeof(Group), request.GroupId);
        
        // If the user's already in the group, throw conflict
        if(group.Users.Any(user => user.Id == request.UserId))
            throw ConflictException.ResourceExists("Membership", request.ToJson());
        
        // Otherwise get the user
        var user = await unitOfWork.Users.GetUserByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(typeof(User), request.UserId);
        
        // If the user doesn't belong to the groups organisation, throw validation exception
        if (!user.IsMemberOfOrganisation(group.OrganisationId))
            throw GroupMembershipException.CrossOrganisationMembership();
        
        // Update from the group side - that's the domain this service controls.
        group.AddUser(user);
        _ = await unitOfWork.Groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {userId} added to group {groupId}", request.UserId, request.GroupId);
    }
}