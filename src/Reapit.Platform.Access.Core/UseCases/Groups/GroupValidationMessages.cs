using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Groups;

/// <summary>Validation messages for <see cref="Group"/> validation.</summary>

public static class GroupValidationMessages
{
    public const string NameExceedsMaximumLength = "Name must be 100 characters or fewer.";
    
    public const string DescriptionExceedsMaximumLength = "Description must be 1000 characters or fewer.";
    
    public const string OrganisationNotFound = "Invalid organisation identifier.";

    public const string NameUnavailable = "Name must be unique within an organisation.";
}