using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;

/// <summary>Handler for the <see cref="CreateGroupCommand"/> request.</summary>
public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Group>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateGroupCommand> _validator;
    private readonly ILogger<CreateGroupCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateGroupCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public CreateGroupCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateGroupCommand> validator,
        ILogger<CreateGroupCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Group> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var group = new Group(request.Name, request.Description, request.OrganisationId);
        _ = await _unitOfWork.Groups.CreateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group created: {id} ({blob})", group.Id, group.AsSerializable());
        return group;
    }
}