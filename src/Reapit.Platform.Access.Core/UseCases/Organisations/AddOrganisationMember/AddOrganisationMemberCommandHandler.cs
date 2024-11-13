using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;

/// <summary>Request handler for the <see cref="AddOrganisationMemberCommand"/> command.</summary>
public class AddOrganisationMemberCommandHandler : IRequestHandler<AddOrganisationMemberCommand, OrganisationUser>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddOrganisationMemberCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="AddOrganisationMemberCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public AddOrganisationMemberCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<AddOrganisationMemberCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // <inheritdoc/>
    public async Task<OrganisationUser> Handle(AddOrganisationMemberCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.OrganisationId, cancellationToken)
            ?? throw new NotFoundException(typeof(Organisation), request.OrganisationId);

        if (organisation.OrganisationUsers.Any(ou => ou.UserId == request.UserId))
            throw ConflictException.ResourceExists("Member", request.UserId);
        
        var user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(typeof(User), request.UserId);

        var organisationUser = new OrganisationUser(organisation, user);

        _ = await _unitOfWork.Organisations.AddMemberAsync(organisationUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {userId} added to organisation {organisationId}", request.UserId, request.OrganisationId);
        return organisationUser;
    }
}