using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.GetOrganisationById;

/// <summary>Request handler for the <see cref="GetOrganisationByIdQuery"/> query.</summary>
public class GetOrganisationByIdQueryHandler : IRequestHandler<GetOrganisationByIdQuery,Organisation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrganisationByIdQueryHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="GetOrganisationByIdQueryHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public GetOrganisationByIdQueryHandler(
        IUnitOfWork unitOfWork, 
        ILogger<GetOrganisationByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Organisation> Handle(GetOrganisationByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving organisation: {id}", request.Id);
        return await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException(typeof(Organisation), request.Id);
    }
}