using FluentValidation;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;

/// <summary>Validator for the <see cref="CreateGroupCommand"/> request.</summary>
public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateGroupCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateGroupCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(GroupValidationMessages.NameExceedsMaximumLength);
        
        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(GroupValidationMessages.DescriptionExceedsMaximumLength);
        
        RuleFor(request => request.OrganisationId)
            .MustAsync(async (organisationId, cancellationToken) 
                => await OrganisationExists(organisationId, cancellationToken))
            .WithMessage(GroupValidationMessages.OrganisationNotFound);
        
        RuleFor(request => request)
            .MustAsync(async (command, cancellationToken) 
                => await IsNameAvailable(command.OrganisationId, command.Name, cancellationToken))
            .WithMessage(GroupValidationMessages.NameUnavailable)
            .WithName(nameof(CreateGroupCommand.Name));
    }

    private async Task<bool> OrganisationExists(string organisationId, CancellationToken cancellationToken)
    {
        var organisation = await _unitOfWork.Organisations.GetOrganisationByIdAsync(organisationId, cancellationToken);
        return organisation != null;
    }

    private async Task<bool> IsNameAvailable(string organisationId, string name, CancellationToken cancellationToken)
    {
        var groups = await _unitOfWork.Groups.GetGroupsAsync(pagination: new PaginationFilter(PageSize: 1), organisationId: organisationId, name: name, cancellationToken: cancellationToken);
        return !groups.Any();
    }
}