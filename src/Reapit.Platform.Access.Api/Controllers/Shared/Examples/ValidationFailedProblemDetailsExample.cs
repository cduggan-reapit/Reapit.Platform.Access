using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Access.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a validation error.</summary>
[ExcludeFromCodeCoverage]
public class ValidationFailedProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://www.reapit.com/errors/validation",
            Detail = "One or more validation errors occurred.",
            Title = "Validation Failed",
            Status = 422,
            Extensions =
            {
                {
                    "errors", new Dictionary<string, string[]>
                    {
                        { "propertyName", ["errorMessage"] }
                    }
                }
            }
        };
}