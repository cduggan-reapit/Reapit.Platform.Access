using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UseCases.Users;

/// <summary>Validation messages for <see cref="User"/> validation.</summary>
public static class UserValidationMessages
{
    public const string IdExceedsMaxLength = "User identifiers must be 100 characters or fewer.";
    
    public const string NameExceedsMaxLength = "User name must be 500 characters or fewer.";
    
    public const string EmailExceedsMaxLength = "User email addresses must be 1000 characters or fewer.";
}