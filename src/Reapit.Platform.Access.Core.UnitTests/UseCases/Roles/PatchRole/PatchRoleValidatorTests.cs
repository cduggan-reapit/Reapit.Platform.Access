using Reapit.Platform.Access.Core.UnitTests.TestHelpers;
using Reapit.Platform.Access.Core.UseCases.Roles;
using Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.PatchRole;

public class PatchRoleValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();

    [Fact]
    public async Task Validation_Succeeds_WhenNameAndDescriptionNull()
    {
        var command = GetRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validation_DoesNotFail_WhenRoleNotFound_AndValuesOtherwiseValid()
    {
        var command = GetRequest(name: "name", description: "description");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validation_Succeeds_WhenRoleFound_AndNameUnchanged()
    {
        var command = GetRequest(name: "same name", description: "new description");
        var role = new Role("same name", "old description");
        
        // The role is found, the name is different, and it doesn't call GetRoles because it's unchanged.
        _roleRepository.GetRoleByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(role);

        _roleRepository.GetRolesAsync(
                userId: null, 
                name: command.Name, 
                description: null, 
                pagination: new PaginationFilter(PageSize: 1),
                dateFilter: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new Role("other role", "which would cause failure")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();

        await _roleRepository.DidNotReceive().GetRolesAsync(
            Arg.Any<string?>(),
            Arg.Any<string?>(), 
            Arg.Any<string?>(), 
            Arg.Any<PaginationFilter?>(), 
            Arg.Any<TimestampFilter?>(),
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validation_Succeeds_WhenRoleFound_AndValuesValid()
    {
        var command = GetRequest(name: "new name", description: "new description");
        var role = new Role("old name", "old description");
        
        // The role is found, the name is different, and nothing else exists with the same name.
        _roleRepository.GetRoleByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(role);

        _roleRepository.GetRolesAsync(
                userId: null, 
                name: command.Name, 
                description: null, 
                pagination: new PaginationFilter(PageSize: 1),
                dateFilter: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Role>());
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validation_Fails_WhenNameTooLong()
    {
        var command = GetRequest(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchRoleCommand.Name), RoleValidationMessages.NameExceedsMaximumLength);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenRoleFound_AndNameUnavailable()
    {
        var command = GetRequest(name: "new name", description: "new description");
        var role = new Role("old name", "old description");
        
        // The role is found, the name is different, and something else exists with the same name.
        _roleRepository.GetRoleByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(role);

        _roleRepository.GetRolesAsync(
                userId: null, 
                name: command.Name, 
                description: null, 
                pagination: new PaginationFilter(PageSize: 1),
                dateFilter: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new Role("other role", "which will cause failure")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchRoleCommand.Name), RoleValidationMessages.NameUnavailable);

    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validation_Fails_WhenDescriptionTooLong()
    {
        var command = GetRequest(description: new string('a', 1001));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchRoleCommand.Description), RoleValidationMessages.DescriptionExceedsMaximumLength);
    }
    
    /*
     * Private methods
     */

    private PatchRoleCommandValidator CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new PatchRoleCommandValidator(_unitOfWork);
    }

    private static PatchRoleCommand GetRequest(string id = "id", string? name = null, string? description = null)
        => new(id, name, description);
}