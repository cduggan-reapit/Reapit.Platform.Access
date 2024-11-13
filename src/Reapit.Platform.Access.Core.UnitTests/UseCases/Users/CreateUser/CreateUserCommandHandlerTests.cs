using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Users.CreateUser;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<CreateUserCommand> _validator = Substitute.For<IValidator<CreateUserCommand>>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<CreateUserCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsConflictException_WhenIdAlreadyExists()
    {
        const string id = "user-id";
        _validator.ValidateAsync(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new User(id, "any", "any"));
        
        var command = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntity_WhenUserCreated()
    {
        var timeFixture = new DateTimeOffset(2024, 11, 13, 8, 43, 17, TimeSpan.Zero);
        const string id = "user-id", name = "user-name", email = "user-email";
        _validator.ValidateAsync(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        using var _ = new DateTimeOffsetProviderContext(timeFixture);
        var command = GetRequest(id: id, name: name, email: email);
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);

        // Check the created (and thus returned) user is constructed correctly ...
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(name);
        actual.Email.Should().Be(email);
        actual.DateLastSynchronised.Should().Be(timeFixture);
        
        // ... and that the right calls were made
        await _userRepository.Received(1).CreateUserAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private CreateUserCommandHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        return new CreateUserCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static CreateUserCommand GetRequest(string id = "id", string name = "name", string email = "email")
        => new(id, name, email);

}