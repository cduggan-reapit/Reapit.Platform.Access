using FluentValidation;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;

/// <summary>Validator for the <see cref="CreateRoleCommand"/> request.</summary>
public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateRoleCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateRoleCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(RoleValidationMessages.NameExceedsMaximumLength);
        
        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(RoleValidationMessages.DescriptionExceedsMaximumLength);
        
        RuleFor(request => request)
            .MustAsync(async (command, cancellationToken) => await IsNameAvailable(command.Name, cancellationToken))
            .WithMessage(RoleValidationMessages.NameUnavailable)
            .WithName(nameof(CreateRoleCommand.Name));
    }
    
    private async Task<bool> IsNameAvailable(string name, CancellationToken cancellationToken)
    {
        var roles = await _unitOfWork.Roles.GetRolesAsync(pagination: new PaginationFilter(PageSize: 1), name: name, cancellationToken: cancellationToken);
        return !roles.Any();
    }
}