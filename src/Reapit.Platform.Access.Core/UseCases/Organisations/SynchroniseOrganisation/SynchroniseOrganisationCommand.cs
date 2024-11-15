using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;

/// <summary>Command to update a Organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
/// <param name="Name">The name of the organisation.</param>
public record SynchroniseOrganisationCommand(string Id, string Name) : IRequest<Organisation>;