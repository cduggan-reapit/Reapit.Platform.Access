using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;

/// <summary>Validator for the <see cref="UpdateUserCommand"/> type.</summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /*
     * | Property | Constraints               |
     * | -------- | ------------------------- |
     * | Id       | Required,                 |
     * | Name     | Required, MaxLength(500)  |
     * | Email    | Required, MaxLength(1000) |
    */

    /// <summary>Initializes a new instance of the <see cref="UpdateUserCommandValidator"/> class.</summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required);
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(500)
            .WithMessage(UserValidationMessages.NameExceedsMaxLength);
        
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(1000)
            .WithMessage(UserValidationMessages.EmailExceedsMaxLength);
    }
}