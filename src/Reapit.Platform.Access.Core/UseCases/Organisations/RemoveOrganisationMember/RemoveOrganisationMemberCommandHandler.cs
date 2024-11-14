using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;

/// <summary>Request handler for the <see cref="RemoveOrganisationMemberCommand"/> command.</summary>
public class RemoveOrganisationMemberCommandHandler : IRequestHandler<RemoveOrganisationMemberCommand, OrganisationUser>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveOrganisationMemberCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="RemoveOrganisationMemberCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public RemoveOrganisationMemberCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<RemoveOrganisationMemberCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // <inheritdoc/>
    public async Task<OrganisationUser> Handle(RemoveOrganisationMemberCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.OrganisationId, cancellationToken)
            ?? throw new NotFoundException(typeof(Organisation), request.OrganisationId);

        var relationship = organisation.OrganisationUsers.SingleOrDefault(membership => membership.UserId == request.UserId)
                   ?? throw new NotFoundException("Member", request.UserId);
        
        _ = await _unitOfWork.Organisations.RemoveMemberAsync(relationship, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {userId} removed from organisation {organisationId}", request.UserId, request.OrganisationId);
        return relationship;
    }
}