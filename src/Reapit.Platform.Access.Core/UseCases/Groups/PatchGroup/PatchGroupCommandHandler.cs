using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;

/// <summary>Handler for the <see cref="PatchGroupCommand"/> request.</summary>
public class PatchGroupCommandHandler : IRequestHandler<PatchGroupCommand, Group>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PatchGroupCommand> _validator;
    private readonly ILogger<PatchGroupCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="PatchGroupCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public PatchGroupCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<PatchGroupCommand> validator,
        ILogger<PatchGroupCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Group> Handle(PatchGroupCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await _unitOfWork.Groups.GetGroupByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Group), request.Id);
        
        // Update the group and return early if there are no changes to apply...
        entity.Update(request.Name, request.Description);
        if (!entity.IsDirty)
            return entity;
        
        // ...otherwise persist the update
        _ = await _unitOfWork.Groups.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Group updated: {id} ({blob})", entity.Id, entity.AsSerializable());
        return entity;
    }
}