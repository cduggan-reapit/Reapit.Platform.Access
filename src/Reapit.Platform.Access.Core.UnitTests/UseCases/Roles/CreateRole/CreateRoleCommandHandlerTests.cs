using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.CreateRole;

public class CreateRoleCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IValidator<CreateRoleCommand> _validator = Substitute.For<IValidator<CreateRoleCommand>>();
    private readonly FakeLogger<CreateRoleCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<CreateRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsCreatedRole_WhenOperationSuccessful()
    {
        _validator.ValidateAsync(Arg.Any<CreateRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        var request = GetRequest();
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);

        actual.Name.Should().Be(request.Name);
        actual.Description.Should().Be(request.Description);

        await _roleRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private CreateRoleCommandHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new CreateRoleCommandHandler(_unitOfWork, _validator, _logger);
    }
    
    private static CreateRoleCommand GetRequest(
        string name = "name", 
        string description = "")
        => new(name, description);
}