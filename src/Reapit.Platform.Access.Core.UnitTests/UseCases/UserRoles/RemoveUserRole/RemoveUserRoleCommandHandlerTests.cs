using Reapit.Platform.Access.Core.UseCases.UserRoles.RemoveUserRole;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.UserRoles.RemoveUserRole;

public class RemoveUserRoleCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly FakeLogger<RemoveUserRoleCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task? Handle_ThrowsNotFound_WhenRoleNotFound()
    {
        const int roleNumber = 7;
        var request = GetRequest(roleNumber);
        
        _roleRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Role?>(null));

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task? Handle_ThrowsNotFound_WhenUserNotInRole()
    {
        const int roleNumber = 7;
        var request = GetRequest(roleNumber);
        var role = GetEntity(roleNumber, null);
        
        _roleRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task? Handle_RemovesUser_WhenUserInRole()
    {
        const int roleNumber = 7;
        var request = GetRequest(roleNumber);
        var role = GetEntity(roleNumber, request.UserId);
        
        _roleRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(role);

        var sut = CreateSut();
        await sut.Handle(request, default);

        await _roleRepository.Received(1).UpdateAsync(Arg.Is<Role>(g => g.Id == role.Id && !role.Users.Any()), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private RemoveUserRoleCommandHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new RemoveUserRoleCommandHandler(_unitOfWork, _logger);
    }

    private static RemoveUserRoleCommand GetRequest(int roleId = 1, string userId = "user-id")
        => new(new Guid($"{roleId:D32}").ToString("N"), userId);

    private static Role GetEntity(int roleId = 1, string? userId = "user-id")
    {
        using var _ = new GuidProviderContext(new Guid($"{roleId:D32}"));
        var role = new Role("name", "description");
        
        if(userId != null)
            role.AddUser(new User(userId, "name", "email"));

        return role;
    }
}