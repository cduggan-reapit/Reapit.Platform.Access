using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Examples;

/// <summary>Example provider for the <see cref="PatchGroupRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchGroupRequestModelExample : IExamplesProvider<PatchGroupRequestModel>
{
    /// <inheritdoc />
    public PatchGroupRequestModel GetExamples()
        => new(
            Name: "Stockton South Office Admins",
            Description: "Users with administrator access to offices in southern Stockton");
}