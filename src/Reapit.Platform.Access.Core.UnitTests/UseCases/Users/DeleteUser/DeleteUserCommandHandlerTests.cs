using Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.DeleteUser;

public class DeleteUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<DeleteUserCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        const string id = "test-id";
        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsDeletedUser_WhenUserFound()
    {
        const string id = "test-id";
        var expected = new User(id, "random", "initial");
        _userRepository.GetUserByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(expected);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);
        
        await _userRepository.Received(1).DeleteUserAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private DeleteUserCommandHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        return new DeleteUserCommandHandler(_unitOfWork, _logger);
    }

    private static DeleteUserCommand GetRequest(string id = "default-id")
        => new(Id: id);
}