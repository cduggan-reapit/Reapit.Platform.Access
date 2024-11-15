using FluentValidation;
using MediatR;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;

/// <summary>Handler for the <see cref="GetGroupsQuery"/> request.</summary>
/// <param name="unitOfWork">The unit of work service.</param>
/// <param name="validator">The request validator.</param>
public class GetGroupsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetGroupsQuery> validator) 
    : IRequestHandler<GetGroupsQuery, IEnumerable<Group>>
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Group>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        
        return await unitOfWork.Groups.GetGroupsAsync(
            request.UserId,
            request.OrganisationId,
            request.Name,
            request.Description,
            pagination,
            dateFilter,
            cancellationToken);
    }
}