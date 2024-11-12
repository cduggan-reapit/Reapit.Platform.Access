using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;

/// <summary>Command to delete a organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
public record DeleteOrganisationCommand(string Id) : IRequest<Organisation>;