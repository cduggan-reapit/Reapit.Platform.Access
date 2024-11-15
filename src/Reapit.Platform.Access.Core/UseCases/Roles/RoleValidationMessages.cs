using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Roles;

/// <summary>Validation messages for <see cref="Role"/> validation.</summary>

public static class RoleValidationMessages
{
    public const string NameExceedsMaximumLength = "Name must be 100 characters or fewer.";

    public const string DescriptionExceedsMaximumLength = "Description must be 1000 characters or fewer.";

    public const string NameUnavailable = "Name must be unique.";
}