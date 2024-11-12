using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;

/// <summary>Validator for the <see cref="UpdateOrganisationCommand"/> type.</summary>
public class UpdateOrganisationCommandValidator : AbstractValidator<UpdateOrganisationCommand>
{
    /*
     * | Property | Constraints    |
     * | -------- | -------------- |
     * | Id       | Required       |
     * | Name     | MaxLength(100) |
    */

    /// <summary>Initializes a new instance of the <see cref="UpdateOrganisationCommandValidator"/> class.</summary>
    public UpdateOrganisationCommandValidator()
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