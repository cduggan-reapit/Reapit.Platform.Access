using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;

/// <summary>Example provider for the <see cref="CreateRoleRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateRoleRequestModelExample : IExamplesProvider<CreateRoleRequestModel>
{
    /// <inheritdoc />
    public CreateRoleRequestModel GetExamples()
        => new("Reapit SuperUsers", "Internal users with elevated privileges.");
}