using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Groups.V1.Examples;

/// <summary>Example provider for the <see cref="GroupModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class GroupModelExample : IExamplesProvider<GroupModel>
{
    /// <inheritdoc />
    public GroupModel GetExamples()
        => new GroupModel(
            "081820fa12de47c3b4f9f71bc3bd1757",
            "AgencyCloud SuperUsers",
            "5c9b4a946be842f69163958b6d97e233",
            new DateTime(2024, 11, 13, 21, 21, 15),
            new DateTime(2024, 11, 14, 8, 54, 26));
}