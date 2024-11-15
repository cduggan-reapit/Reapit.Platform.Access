using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;

/// <summary>Validator for the <see cref="GetRolesQuery"/> request.</summary>
public class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetRolesQueryValidator"/> class.</summary>
    public GetRolesQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}