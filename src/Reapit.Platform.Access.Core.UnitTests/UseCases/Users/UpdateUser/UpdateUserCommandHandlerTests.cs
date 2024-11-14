using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<UpdateUserCommand> _validator = Substitute.For<IValidator<UpdateUserCommand>>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<UpdateUserCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        const string id = "user-id";
        _validator.ValidateAsync(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));
        
        
        var command = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntity_WhenUserUpdated()
    {
        var timeFixture = new DateTimeOffset(2024, 11, 13, 8, 43, 17, TimeSpan.Zero);
        const string id = "user-id", name = "user-name", email = "user-email";

        // Create a user with a sync date at the epoch
        using var initialContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var user = new User(id, "initial", "initial");
        
        _validator.ValidateAsync(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(user);

        using var _ = new DateTimeOffsetProviderContext(timeFixture);
        var command = GetRequest(id: id, name: name, email: email);
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);

        // Check the created (and thus returned) user is constructed correctly ...
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(name);
        actual.Email.Should().Be(email);
        actual.DateLastSynchronised.Should().Be(timeFixture);
        
        // ... and that the right calls were made (after the domain entity was updated)
        await _userRepository.Received(1).UpdateUserAsync(Arg.Is<User>(u => u.Name == name), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private UpdateUserCommandHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        return new UpdateUserCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static UpdateUserCommand GetRequest(string id = "id", string name = "name", string email = "email")
        => new(id, name, email);

}