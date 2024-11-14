using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Examples;

/// <summary>Example provider for a ResultPage containing GroupModels.</summary>
[ExcludeFromCodeCoverage]
public class GroupModelResultPageExample : IExamplesProvider<ResultPage<GroupModel>>
{
    /// <inheritdoc />
    public ResultPage<GroupModel> GetExamples()
        => new([new GroupModelExample().GetExamples()], 1, 1729607077475047);
}