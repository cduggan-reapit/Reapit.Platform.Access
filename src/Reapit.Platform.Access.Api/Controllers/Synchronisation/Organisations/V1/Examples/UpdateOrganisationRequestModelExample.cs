using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Organisations.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Organisations.V1.Examples;

/// <summary>Example provider for the <see cref="UpdateOrganisationRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UpdateOrganisationRequestModelExample : IExamplesProvider<UpdateOrganisationRequestModel>
{
    /// <inheritdoc />
    public UpdateOrganisationRequestModel GetExamples()
        => new("Reapit");
}