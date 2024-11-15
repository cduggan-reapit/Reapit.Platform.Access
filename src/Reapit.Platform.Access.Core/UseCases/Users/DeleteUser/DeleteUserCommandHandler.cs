using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;

/// <summary>Command handler for the <see cref="DeleteUserCommand"/> type.</summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public DeleteUserCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<DeleteUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<User> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(User), request.Id);
        
        _ = await _unitOfWork.Users.DeleteUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User deleted: {id} ({json})", user.Id, user.ToString());
        return user;
    }
}