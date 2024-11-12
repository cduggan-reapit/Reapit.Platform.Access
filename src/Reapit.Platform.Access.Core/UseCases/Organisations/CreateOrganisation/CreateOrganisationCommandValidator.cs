using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;

/// <summary>Validator for the <see cref="CreateOrganisationCommand"/> type.</summary>
public class CreateOrganisationCommandValidator : AbstractValidator<CreateOrganisationCommand>
{
    /*
     * | Property | Constraints              |
     * | -------- | ------------------------ |
     * | Id       | Required, MaxLength(100) |
     * | Name     | Required, MaxLength(100) |
    */

    /// <summary>Initializes a new instance of the <see cref="CreateOrganisationCommandValidator"/> class.</summary>
    public CreateOrganisationCommandValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(OrganisationValidationMessages.IdExceedsMaxLength);
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(OrganisationValidationMessages.NameExceedsMaxLength);
    }
}