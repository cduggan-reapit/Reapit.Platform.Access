﻿using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.Extensions;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.CreateUser;

/// <summary>Command handler for the <see cref="CreateUserCommand"/> type.</summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.GetUserByIdAsync(request.Id, cancellationToken) != null)
            throw ConflictException.ResourceExists(nameof(User), request.Id);
        
        var user = new User(request.Id, request.Name, request.Email);
        
        _ = await _unitOfWork.Users.CreateUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created: {id} ({json})", user.Id, user.AsSerializable().ToJson());
        return user;
    }
}