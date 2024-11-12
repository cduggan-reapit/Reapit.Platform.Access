using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Users.CreateUser;

/// <summary>Validator for the <see cref="CreateUserCommand"/> type.</summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /*
     * | Property | Constraints              |
     * | -------- | ------------------------ |
     * | Id       | Required, MaxLength(36)  |
     * | Name     | Required, MaxLength(100) |
     * | Email    | Required, MaxLength(500) |
    */

    /// <summary>Initializes a new instance of the <see cref="CreateUserCommandValidator"/> class.</summary>
    public CreateUserCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(36)
            .WithMessage(UserValidationMessages.IdExceedsMaxLength);
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(UserValidationMessages.NameExceedsMaxLength);
        
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(500)
            .WithMessage(UserValidationMessages.EmailExceedsMaxLength);
    }
}