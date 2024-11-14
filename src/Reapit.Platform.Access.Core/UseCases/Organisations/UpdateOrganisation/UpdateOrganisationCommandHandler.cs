using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;

/// <summary>Command handler for the <see cref="UpdateOrganisationCommand"/> type.</summary>
public class UpdateOrganisationCommandHandler : IRequestHandler<UpdateOrganisationCommand,Organisation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateOrganisationCommand> _validator;
    private readonly ILogger<UpdateOrganisationCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="UpdateOrganisationCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public UpdateOrganisationCommandHandler(
        IUnitOfWork unitOfWork, 
        IValidator<UpdateOrganisationCommand> validator,
        ILogger<UpdateOrganisationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<Organisation> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Organisation), request.Id);
        
        organisation.Update(request.Name);

        _ = await _unitOfWork.Organisations.UpdateOrganisationAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organisation updated: {id} ({json})", organisation.Id, organisation.AsSerializable().ToJson());
        return organisation;
    }
}