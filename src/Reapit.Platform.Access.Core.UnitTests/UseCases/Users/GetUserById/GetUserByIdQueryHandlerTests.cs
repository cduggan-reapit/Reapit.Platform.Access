using Reapit.Platform.Access.Core.UseCases.Users.GetUserById;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Users.GetUserById;

public class GetUserByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<GetUserByIdQueryHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async void Handle_ThrowsNotFound_WhenUserNotFound()
    {
        _userRepository.GetUserByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));
        
        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async void Handle_ReturnsEntity_WhenUserFound()
    {
        const string id = "user-id";
        var expected = new User(id, "arbitrary-name", "arbitrary-email");
        _userRepository.GetUserByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(expected);
        
        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private GetUserByIdQueryHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        return new GetUserByIdQueryHandler(_unitOfWork, _logger);
    }

    private static GetUserByIdQuery GetRequest(string id = "default-id")
        => new(Id: id);
}