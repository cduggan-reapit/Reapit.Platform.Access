using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;

/// <summary>Handler for the <see cref="CreateRoleCommand"/> request.</summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRoleCommand> _validator;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateRoleCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public CreateRoleCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateRoleCommand> validator,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Role> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var role = new Role(request.Name, request.Description);
        _ = await _unitOfWork.Roles.CreateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Role created: {id} ({blob})", role.Id, role.AsSerializable());
        return role;
    }
}