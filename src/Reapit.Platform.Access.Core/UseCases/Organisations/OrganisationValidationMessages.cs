using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Organisations;

/// <summary>Validation messages for <see cref="Organisation"/> validation.</summary>
public static class OrganisationValidationMessages
{
    public const string IdExceedsMaxLength = "Organisation identifiers must be 36 characters or fewer.";
    
    public const string NameExceedsMaxLength = "Organisation name must be 100 characters or fewer.";
}