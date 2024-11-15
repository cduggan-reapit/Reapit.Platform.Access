using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Organisations.V1.Examples;

/// <summary>Example provider for the <see cref="SynchroniseOrganisationRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UpdateOrganisationRequestModelExample : IExamplesProvider<SynchroniseOrganisationRequestModel>
{
    /// <inheritdoc />
    public SynchroniseOrganisationRequestModel GetExamples()
        => new("Reapit");
}