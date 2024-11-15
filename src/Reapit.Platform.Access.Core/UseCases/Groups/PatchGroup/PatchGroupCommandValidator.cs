using FluentValidation;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;

/// <summary>Validator for the <see cref="PatchGroupCommand"/> request.</summary>
public class PatchGroupCommandValidator : AbstractValidator<PatchGroupCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="PatchGroupCommand"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public PatchGroupCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(request => request.Name)
            .MaximumLength(100)
            .WithMessage(GroupValidationMessages.NameExceedsMaximumLength);

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(GroupValidationMessages.DescriptionExceedsMaximumLength);

        // Only run this when the name is provided - check that it's unique!
        RuleFor(request => request)
            .MustAsync(async (request, cancellationToken) => await IsNameUnique(request, cancellationToken))
            .When(request => request.Name != null)
            .WithMessage(GroupValidationMessages.NameUnavailable)
            .WithName(nameof(PatchGroupCommand.Name));
    }

    /// <summary>Checks to see if the requested name is currently in use by any other groups within the organisation.</summary>
    /// <returns>True if the name is available; otherwise false.</returns>
    private async Task<bool> IsNameUnique(PatchGroupCommand command, CancellationToken cancellationToken)
    {
        // If the group doesn't exist, return true. Validation should pass and allow the handler to throw NotFound.
        var subject = await _unitOfWork.Groups.GetGroupByIdAsync(command.Id, cancellationToken);
        if (subject is null)
            return true;

        // If the name's unchanged then there's no need to check again.
        if (subject.Name == command.Name)
            return true;
        
        // If any records exist with the requested name, then it is not unique and we return false.
        var others = await _unitOfWork.Groups.GetGroupsAsync(
            organisationId: subject.OrganisationId, 
            name: command.Name, 
            pageSize: 1,
            cancellationToken: cancellationToken);

        return !others.Any();
    }
}