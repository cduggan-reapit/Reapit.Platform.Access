using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;

/// <summary>Handler for the <see cref="PatchRoleCommand"/> request.</summary>
public class PatchRoleCommandHandler : IRequestHandler<PatchRoleCommand, Role>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PatchRoleCommand> _validator;
    private readonly ILogger<PatchRoleCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="PatchRoleCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public PatchRoleCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<PatchRoleCommand> validator,
        ILogger<PatchRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Role> Handle(PatchRoleCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await _unitOfWork.Roles.GetRoleByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Role), request.Id);
        
        // Update the role and return early if there are no changes to apply...
        entity.Update(request.Name, request.Description);
        if (!entity.IsDirty)
            return entity;
        
        // ...otherwise persist the update
        _ = await _unitOfWork.Roles.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Role updated: {id} ({blob})", entity.Id, entity.AsSerializable());
        return entity;
    }
}