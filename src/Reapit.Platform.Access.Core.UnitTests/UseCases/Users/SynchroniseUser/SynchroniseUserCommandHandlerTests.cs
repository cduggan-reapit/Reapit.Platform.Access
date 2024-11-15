using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Users.SynchroniseUser;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.SynchroniseUser;

public class SynchroniseUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<SynchroniseUserCommand> _validator = Substitute.For<IValidator<SynchroniseUserCommand>>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<SynchroniseUserCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<SynchroniseUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_CreatesUser_WhenUserDoesNotExist()
    {
        var timeFixture = new DateTimeOffset(2024, 11, 13, 8, 43, 17, TimeSpan.Zero);
        const string id = "user-id", name = "user-name", email = "user-email";
        _validator.ValidateAsync(Arg.Any<SynchroniseUserCommand>(), Arg.Any<CancellationToken>())
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
    
    [Fact]
    public async Task Handle_UpdatesUser_WhenUserAlreadyExists()
    {
        const string id = "user-id";
        _validator.ValidateAsync(Arg.Any<SynchroniseUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new User(id, "any", "any"));
        
        var command = GetRequest(id: id);
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);

        // Check the request is applied constructed correctly ...
        actual.Name.Should().Be(command.Name);
        actual.Email.Should().Be(command.Email);
        
        // ... and that the right calls were made
        await _userRepository.Received(1).UpdateUserAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private SynchroniseUserCommandHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        return new SynchroniseUserCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static SynchroniseUserCommand GetRequest(string id = "id", string name = "name", string email = "email")
        => new(id, name, email);

}