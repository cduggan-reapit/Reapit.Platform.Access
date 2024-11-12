using FluentValidation.Results;
using Reapit.Platform.Access.Core.UnitTests.TestHelpers;

namespace Reapit.Platform.Organisations.Core.UnitTests.TestHelpers;

public static class ValidationResultExtensions
{
    public static ValidationResultAssertions Should(this ValidationResult instance)
        => new(instance); 
}