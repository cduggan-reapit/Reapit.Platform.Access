using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.OrganisationUsers.DeleteOrganisationUser;

/// <summary>Request handler for the <see cref="DeleteOrganisationUserCommand"/> command.</summary>
public class DeleteOrganisationUserCommandHandler : IRequestHandler<DeleteOrganisationUserCommand, OrganisationUser>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrganisationUserCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="DeleteOrganisationUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public DeleteOrganisationUserCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<DeleteOrganisationUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // <inheritdoc/>
    public async Task<OrganisationUser> Handle(DeleteOrganisationUserCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _unitOfWork.OrganisationUsers.GetRelationshipAsync(
            organisationId: request.OrganisationId, 
            userId: request.UserId, 
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Organisation-User Relationship", request.ToJson());
        
        _ = await _unitOfWork.OrganisationUsers.DeleteRelationshipAsync(relationship, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return relationship;
    }
}