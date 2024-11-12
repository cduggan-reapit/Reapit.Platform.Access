using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.GetOrganisationById;

/// <summary>Request to retrieve details of an individual organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
public record GetOrganisationByIdQuery(string Id) : IRequest<Organisation>;