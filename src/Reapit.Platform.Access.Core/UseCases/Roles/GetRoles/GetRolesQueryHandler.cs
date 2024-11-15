using FluentValidation;
using MediatR;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;

/// <summary>Handler for the <see cref="GetRolesQuery"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
/// <param name="validator">The request validator.</param>
public class GetRolesQueryHandler(IUnitOfWork unitOfWork, IValidator<GetRolesQuery> validator) 
    : IRequestHandler<GetRolesQuery, IEnumerable<Role>>
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Role>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var timestamps = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        
        return await unitOfWork.Roles.GetRolesAsync(
            request.UserId,
            request.Name,
            request.Description,
            pagination,
            timestamps,
            cancellationToken);
    }
}