using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Synchronisation.Users.V1.Examples;

/// <summary>Example provider for the <see cref="CreateUserRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateUserRequestModelExample : IExamplesProvider<CreateUserRequestModel>
{
    /// <inheritdoc />
    public CreateUserRequestModel GetExamples()
        => new("bfb6d52337c2af70830c8e2c24e64b05", "Joe Bloggs", "jbloggs@reapit.com");
}