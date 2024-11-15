using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;

/// <summary>Command handler for the <see cref="SynchroniseOrganisationCommand"/> type.</summary>
public class SynchroniseOrganisationCommandHandler(
    IUnitOfWork unitOfWork, 
    IValidator<SynchroniseOrganisationCommand> validator,
    ILogger<SynchroniseOrganisationCommandHandler> logger) 
    : IRequestHandler<SynchroniseOrganisationCommand,Organisation>
{
    /// <inheritdoc/>
    public async Task<Organisation> Handle(SynchroniseOrganisationCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var organisation = await unitOfWork.Organisations.GetOrganisationByIdAsync(request.Id, cancellationToken);

        if (organisation == null)
        {
            organisation = new Organisation(request.Id, request.Name);
            await unitOfWork.Organisations.CreateOrganisationAsync(organisation, cancellationToken);
        }
        else
        {
            organisation.Update(request.Name);
            _ = await unitOfWork.Organisations.UpdateOrganisationAsync(organisation, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Organisation synchronised: {id} ({json})", organisation.Id, organisation.AsSerializable().ToJson());
        
        return organisation;
    }
}