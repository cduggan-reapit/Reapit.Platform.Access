using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;

/// <summary>Example provider for the <see cref="PatchRoleRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchRoleRequestModelExample : IExamplesProvider<PatchRoleRequestModel>
{
    /// <inheritdoc />
    public PatchRoleRequestModel GetExamples()
        => new("Reapit SuperUsers", "Internal users with elevated privileges.");
}