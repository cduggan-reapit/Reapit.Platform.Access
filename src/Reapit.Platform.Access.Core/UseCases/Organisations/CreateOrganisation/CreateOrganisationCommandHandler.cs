using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;

/// <summary>Command handler for the <see cref="CreateOrganisationCommand"/> type.</summary>
public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, Organisation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateOrganisationCommand> _validator;
    private readonly ILogger<CreateOrganisationCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateOrganisationCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator;</param>
    /// <param name="logger">The logging service.</param>
    public CreateOrganisationCommandHandler(
        IUnitOfWork unitOfWork, 
        IValidator<CreateOrganisationCommand> validator,
        ILogger<CreateOrganisationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<Organisation> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        if (await _unitOfWork.Organisations.GetOrganisationByIdAsync(request.Id, cancellationToken) != null)
            throw ConflictException.ResourceExists(nameof(Organisation), request.Id);
        
        var organisation = new Organisation(request.Id, request.Name);
        
        _ = await _unitOfWork.Organisations.CreateOrganisationAsync(organisation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organisation created: {id} ({json})", organisation.Id, organisation.AsSerializable().ToJson());
        return organisation;
    }
}