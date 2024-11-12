using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.OrganisationUsers.CreateOrganisationUser;

/// <summary>Request handler for the <see cref="CreateOrganisationUserCommand"/> command.</summary>
public class CreateOrganisationUserCommandHandler : IRequestHandler<CreateOrganisationUserCommand, OrganisationUser>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrganisationUserCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateOrganisationUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public CreateOrganisationUserCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<CreateOrganisationUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // <inheritdoc/>
    public async Task<OrganisationUser> Handle(CreateOrganisationUserCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.OrganisationId, cancellationToken)
            ?? throw new NotFoundException(typeof(Organisation), request.OrganisationId);
        
        var user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(typeof(User), request.UserId);

        var organisationUser = new OrganisationUser(organisation, user);

        _ = await _unitOfWork.OrganisationUsers.CreateRelationshipAsync(organisationUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return organisationUser;
    }
}