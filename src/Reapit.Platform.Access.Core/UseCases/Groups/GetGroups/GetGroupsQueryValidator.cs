using FluentValidation;

namespace Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;

/// <summary>Validator for the <see cref="GetGroupsQuery"/> request.</summary>
public class GetGroupsQueryValidator : AbstractValidator<GetGroupsQuery>
{
    public GetGroupsQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}