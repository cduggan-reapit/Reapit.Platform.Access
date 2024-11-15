using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;

/// <summary>Example provider for a ResultPage containing GroupModels.</summary>
[ExcludeFromCodeCoverage]
public class RoleModelResultPageExample : IExamplesProvider<ResultPage<RoleModel>>
{
    /// <inheritdoc />
    public ResultPage<RoleModel> GetExamples()
        => new([new RoleModelExample().GetExamples()], 1, 1729607077475047);
}