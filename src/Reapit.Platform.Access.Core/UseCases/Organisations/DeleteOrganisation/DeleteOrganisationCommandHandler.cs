using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;

/// <summary>Command handler for the <see cref="DeleteOrganisationCommand"/> type.</summary>
public class DeleteOrganisationCommandHandler : IRequestHandler<DeleteOrganisationCommand,Organisation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrganisationCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="DeleteOrganisationCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public DeleteOrganisationCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<DeleteOrganisationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<Organisation> Handle(DeleteOrganisationCommand request, CancellationToken cancellationToken)
    {
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Organisation), request.Id);
        
        _ = await _unitOfWork.Organisations.DeleteOrganisationAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organisation deleted: {id} ({json})", organisation.Id, organisation.AsSerializable().ToJson());
        return organisation;
    }
}