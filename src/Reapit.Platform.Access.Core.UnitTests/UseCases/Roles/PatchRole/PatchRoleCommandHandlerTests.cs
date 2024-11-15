using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.PatchRole;

public class PatchRoleCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IValidator<PatchRoleCommand> _validator = Substitute.For<IValidator<PatchRoleCommand>>();
    private readonly FakeLogger<PatchRoleCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        ConfigureValidation(false);

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowNotFoundException_WhenRoleNotFound()
    {
        const string id = "test-id";
        
        ConfigureValidation(true);

        _roleRepository.GetRoleByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Role?>(null));

        var request = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsWithoutUpdate_WhenEntityUnchanged()
    {
        var role = new Role("name", "description");
        
        ConfigureValidation(true);

        _roleRepository.GetRoleByIdAsync(role.Id, Arg.Any<CancellationToken>())
            .Returns(role);

        var request = GetRequest(id: role.Id, name: role.Name, description: role.Description);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(role);

        await _roleRepository.DidNotReceive().UpdateAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_ReturnsAfterUpdate_WhenChanged()
    {
        var role = new Role("name", "description");
        
        ConfigureValidation(true);

        _roleRepository.GetRoleByIdAsync(role.Id, Arg.Any<CancellationToken>())
            .Returns(role);

        var request = GetRequest(id: role.Id, name: "new name", description: null);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(role);

        await _roleRepository.Received(1).UpdateAsync(Arg.Is<Role>(roleParam => roleParam.Name == request.Name), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchRoleCommandHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new PatchRoleCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static PatchRoleCommand GetRequest(string id = "id", string? name = "name", string? description = "description")
        => new(id, name, description);

    private void ConfigureValidation(bool isSuccessful)
    {
        var result =  isSuccessful
            ? new ValidationResult()
            : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]);

        _validator.ValidateAsync(Arg.Any<PatchRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);
    }
}