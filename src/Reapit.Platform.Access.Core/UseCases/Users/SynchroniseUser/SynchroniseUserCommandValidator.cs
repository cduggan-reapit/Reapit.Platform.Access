using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Users.SynchroniseUser;

/// <summary>Validator for the <see cref="SynchroniseUserCommand"/> request.</summary>
public class SynchroniseUserCommandValidator : AbstractValidator<SynchroniseUserCommand>
{
    /// <summary>Initializes an instance of the <see cref="SynchroniseUserCommandValidator"/> class.</summary>
    public SynchroniseUserCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(UserValidationMessages.IdExceedsMaxLength);
        
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