using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Access.Core.UseCases.Users.CreateUser;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.Users.GetUserById;

/// <summary>Request handler for the <see cref="GetUserByIdQuery"/> query.</summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="CreateUserCommandHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="logger">The logging service.</param>
    public GetUserByIdQueryHandler(
        IUnitOfWork unitOfWork, 
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving user: {id}", request.Id);
        return await _unitOfWork.Users.GetUserByIdAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException(typeof(User), request.Id);
    }
}