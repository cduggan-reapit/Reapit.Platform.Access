using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;

/// <summary>Validator for the <see cref="SynchroniseOrganisationCommand"/> type.</summary>
public class SynchroniseOrganisationCommandValidator : AbstractValidator<SynchroniseOrganisationCommand>
{
    /*
     * | Property | Constraints    |
     * | -------- | -------------- |
     * | Id       | Required       |
     * | Name     | MaxLength(100) |
    */

    /// <summary>Initializes a new instance of the <see cref="SynchroniseOrganisationCommandValidator"/> class.</summary>
    public SynchroniseOrganisationCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required);
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(OrganisationValidationMessages.NameExceedsMaxLength);
    }
}