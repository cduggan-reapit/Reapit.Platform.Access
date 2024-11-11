using MediatR;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Dummies.GetDummies;

/// <summary>Defines the handler for the <see cref="GetDummiesQuery"/> request.</summary>
public class GetDummiesQueryHandler : IRequestHandler<GetDummiesQuery, IEnumerable<Dummy>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="GetDummiesQueryHandler"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public GetDummiesQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    /// <inheritdoc/>
    public async Task<IEnumerable<Dummy>> Handle(GetDummiesQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Dummies.GetAsync(
            name: request.Name, 
            createdFrom: request.CreatedFrom, 
            createdTo: request.CreatedTo, 
            modifiedFrom: request.ModifiedFrom, 
            modifiedTo: request.ModifiedTo, 
            cancellationToken: cancellationToken);
}