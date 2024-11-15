using FluentValidation;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;

/// <summary>Validator for the <see cref="PatchRoleCommand"/> request.</summary>
public class PatchRoleCommandValidator : AbstractValidator<PatchRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="PatchRoleCommand"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public PatchRoleCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(request => request.Name)
            .MaximumLength(100)
            .WithMessage(RoleValidationMessages.NameExceedsMaximumLength);

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(RoleValidationMessages.DescriptionExceedsMaximumLength);

        // Only run this when the name is provided - check that it's unique!
        RuleFor(request => request)
            .MustAsync(async (request, cancellationToken) => await IsNameUnique(request, cancellationToken))
            .When(request => request.Name != null)
            .WithMessage(RoleValidationMessages.NameUnavailable)
            .WithName(nameof(PatchRoleCommand.Name));
    }

    /// <summary>Checks to see if the requested name is currently in use by any other roles.</summary>
    /// <returns>True if the name is available; otherwise false.</returns>
    private async Task<bool> IsNameUnique(PatchRoleCommand command, CancellationToken cancellationToken)
    {
        // If the role doesn't exist, return true. Validation should pass and allow the handler to throw NotFound.
        var subject = await _unitOfWork.Roles.GetRoleByIdAsync(command.Id, cancellationToken);
        if (subject is null)
            return true;

        // If the name's unchanged then there's no need to check again.
        if (subject.Name == command.Name)
            return true;
        
        // If any records exist with the requested name, then it is not unique and we return false.
        var others = await _unitOfWork.Roles.GetRolesAsync(
            name: command.Name, 
            pagination: new PaginationFilter(PageSize: 1),
            cancellationToken: cancellationToken);

        return !others.Any();
    }
}