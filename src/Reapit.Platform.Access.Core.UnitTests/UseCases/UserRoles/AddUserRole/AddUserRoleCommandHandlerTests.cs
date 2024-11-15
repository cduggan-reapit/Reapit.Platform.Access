using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.UserRoles.AddUserRole;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.UserRoles.AddUserRole;

public class AddUserRoleCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<AddUserRoleCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRoleNotFound()
    {
        _roleRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Role?>(null));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsConflict_WhenUserAlreadyLinked()
    {
        var request = GetRequest();
        var role = new Role("name", "description");
        role.AddUser(new User(request.UserId, "name", "email"));
        
        _roleRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(role);
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserNotFound()
    {
        var request = GetRequest();
        var role = new Role("name", "description");
        
        _roleRepository.GetByIdAsync(request.RoleId, Arg.Any<CancellationToken>())
            .Returns(role);

        _userRepository.GetUserByIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AddsUserToRole_WhenRequestValid()
    {
        var request = GetRequest();
        var role = new Role("name", "description");
        
        _roleRepository.GetByIdAsync(request.RoleId, Arg.Any<CancellationToken>())
            .Returns(role);

        _userRepository.GetUserByIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(new User(request.UserId, "name", "email"));
        
        var sut = CreateSut();
        await sut.Handle(request, default);
        
        await _roleRepository.Received(1).UpdateAsync(Arg.Is<Role>(g => g.Id == role.Id && role.Users.Count == 1), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private AddUserRoleCommandHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        _unitOfWork.Users.Returns(_userRepository);
        return new AddUserRoleCommandHandler(_unitOfWork, _logger);
    }

    private static AddUserRoleCommand GetRequest(int roleId = 1, string userId = "user-id")
        => new(new Guid($"{roleId:D32}").ToString("N"), userId);
}