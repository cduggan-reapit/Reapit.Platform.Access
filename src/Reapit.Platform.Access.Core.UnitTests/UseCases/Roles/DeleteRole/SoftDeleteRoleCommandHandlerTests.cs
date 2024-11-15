using Reapit.Platform.Access.Core.UseCases.Roles.DeleteRole;
using Reapit.Platform.Access.Data.Repositories.Roles;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Roles.DeleteRole;

public class SoftDeleteRoleCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly FakeLogger<SoftDeleteRoleCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRoleNotFound()
    {
        var sut = CreateSut();
        var action = () => sut.Handle(new SoftDeleteRoleCommand("not-found"), default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UpdatesDeletedDate_WhenRoleSoftDeleted()
    {
        var role = new Role("old name", "old description");

        _roleRepository.GetByIdAsync(role.Id, Arg.Any<CancellationToken>())
            .Returns(role);
        
        var sut = CreateSut();
        var actual = await sut.Handle(new SoftDeleteRoleCommand(role.Id), default);
        actual.DateDeleted.Should().NotBeNull();

        await _roleRepository.Received(1).UpdateAsync(Arg.Is<Role>(roleParam => roleParam.Id == role.Id && roleParam.DateDeleted != null), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private SoftDeleteRoleCommandHandler CreateSut()
    {
        _unitOfWork.Roles.Returns(_roleRepository);
        return new SoftDeleteRoleCommandHandler(_unitOfWork, _logger);
    }
}