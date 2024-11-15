using Reapit.Platform.Access.Core.UnitTests.TestHelpers;
using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Roles;
using Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.CreateRole;

public class CreateRoleCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();

    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }

    /*
     * Name
     */
    
    [Fact] 
    public async Task Validate_Fails_WhenNameIsEmpty()
    {
        var request = GetRequest(name: string.Empty);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateRoleCommand.Name), CommonValidationMessages.Required);
    }
    
    [Fact] 
    public async Task Validate_Fails_WhenNameExceedsMaximumLength()
    {
        var request = GetRequest(name: new string('a', 101));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateRoleCommand.Name), RoleValidationMessages.NameExceedsMaximumLength);
    }
    
    [Fact] 
    public async Task Validate_Fails_WhenNameAlreadyTaken()
    {
        var request = GetRequest(name: "name");
        
        _roleRepository.GetRolesAsync(null, request.Name, null, new PaginationFilter(PageSize: 1), null, Arg.Any<CancellationToken>())
            .Returns([new Role("name", "description")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateRoleCommand.Name), RoleValidationMessages.NameUnavailable);
    }
    
    /*
     * Description
     */
    
    [Fact] 
    public async Task Validate_Fails_WhenDescriptionExceedsMaximumLength()
    {
        var request = GetRequest(description: new string('a', 1001));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateRoleCommand.Description), RoleValidationMessages.DescriptionExceedsMaximumLength);
    }
    
    /*
     * Private methods
     */

    private CreateRoleCommandValidator CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new CreateRoleCommandValidator(_unitOfWork);
    }

    private static CreateRoleCommand GetRequest(
        string name = "name", 
        string description = "")
        => new(name, description);
}