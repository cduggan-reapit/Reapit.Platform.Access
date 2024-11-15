using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Roles.V1.Examples;

/// <summary>Example provider for a RoleModel.</summary>
[ExcludeFromCodeCoverage]
public class RoleModelExample : IExamplesProvider<RoleModel>
{
    /// <inheritdoc />
    public RoleModel GetExamples()
        => new(
            Id: "1e55583860d743deac460ca79e5449c7",
            Name: "Reapit Admins",
            DateCreated: DateTime.SpecifyKind(new DateTime(2020, 1, 12, 15, 17, 18), DateTimeKind.Utc),
            DateModified: DateTime.SpecifyKind(new DateTime(2024, 11, 15, 10, 52, 24), DateTimeKind.Utc));
}