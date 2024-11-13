using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;

/// <summary>Command handler for the <see cref="UpdateUserCommand"/> type.</summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logging service.</param>
    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<UpdateUserCommand> validator,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var user = await _unitOfWork.Users.GetUserByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(User), request.Id);
        
        user.Update(request.Name, request.Email);

        _ = await _unitOfWork.Users.UpdateUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated: {id} ({json})", user.Id, user.AsSerializable().ToJson());
        return user;
    }
}