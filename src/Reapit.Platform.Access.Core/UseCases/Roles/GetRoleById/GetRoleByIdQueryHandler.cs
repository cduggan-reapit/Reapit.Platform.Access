using MediatR;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;

/// <summary>Handler for the <see cref="GetRoleByIdQuery"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
public class GetRoleByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoleByIdQuery, Role>
{
    /// <inheritdoc/>
    public async Task<Role> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(typeof(Role), request.Id);
}