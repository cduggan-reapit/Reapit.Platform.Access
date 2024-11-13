using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Examples;

/// <summary>Example provider for the <see cref="SimpleOrganisationModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class OrganisationModelExample : IExamplesProvider<SimpleOrganisationModel>
{
    /// <inheritdoc/>
    public SimpleOrganisationModel GetExamples()
        => new(
            Id: "012efef6dc9e55168d1f15511a9491c6", 
            Name: "Reapit", 
            DateLastSynchronised: new DateTime(2024, 11, 13, 9, 10, 15));
}