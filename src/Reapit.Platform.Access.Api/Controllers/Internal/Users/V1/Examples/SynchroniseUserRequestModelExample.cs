using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Examples;

/// <summary>Example provider for the <see cref="SynchroniseUserRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class UpdateUserRequestModelExample : IExamplesProvider<SynchroniseUserRequestModel>
{
    /// <inheritdoc />
    public SynchroniseUserRequestModel GetExamples()
        => new("Joe Bloggs", "jbloggs@reapit.com");
}