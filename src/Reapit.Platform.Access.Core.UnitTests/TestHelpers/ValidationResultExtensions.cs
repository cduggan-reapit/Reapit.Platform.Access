using FluentValidation.Results;

namespace Reapit.Platform.Access.Core.UnitTests.TestHelpers;

public static class ValidationResultExtensions
{
    public static ValidationResultAssertions Should(this ValidationResult instance)
        => new(instance); 
}