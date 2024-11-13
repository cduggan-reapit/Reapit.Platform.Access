using MediatR;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;

/// <summary>Handler for the <see cref="GetGroupByIdQuery"/> request.</summary>
public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, Group>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="GetGroupByIdQueryHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public GetGroupByIdQueryHandler(IUnitOfWork unitOfWork) 
        => _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<Group> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Groups.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(nameof(Group), request.Id);
}