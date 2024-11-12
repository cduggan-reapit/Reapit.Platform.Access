using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;

/// <summary>Validator for the <see cref="UpdateUserCommand"/> type.</summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /*
     * | Property | Constraints             |
     * | -------- | ----------------------- |
     * | Id       | Required,               |
     * | Name     | MaxLength(100)          |
     * | Email    | MaxLength(500)          |
    */

    /// <summary>Initializes a new instance of the <see cref="UpdateUserCommandValidator"/> class.</summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required);
        
        RuleFor(request => request.Name)
            .MaximumLength(100)
            .WithMessage(UserValidationMessages.NameExceedsMaxLength);
        
        RuleFor(request => request.Email)
            .MaximumLength(500)
            .WithMessage(UserValidationMessages.EmailExceedsMaxLength);
    }
}