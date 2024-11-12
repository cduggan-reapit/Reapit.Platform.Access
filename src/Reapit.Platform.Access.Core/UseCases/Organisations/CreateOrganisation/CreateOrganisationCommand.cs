using MediatR;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;

/// <summary>Command to create a organisation.</summary>
/// <param name="Id">The unique identifier of the organisation.</param>
/// <param name="Name">The name of the organisation.</param>
public record CreateOrganisationCommand(string Id, string Name) : IRequest<Organisation>;