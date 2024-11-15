using Reapit.Platform.Access.Core.UseCases.Groups.DeleteGroup;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.DeleteGroup;

public class SoftDeleteGroupCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly FakeLogger<SoftDeleteGroupCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenGroupNotFound()
    {
        var sut = CreateSut();
        var action = () => sut.Handle(new SoftDeleteGroupCommand("not-found"), default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UpdatesDeletedDate_WhenGroupSoftDeleted()
    {
        var group = new Group("old name", "old description", "organisation-id");

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>())
            .Returns(group);
        
        var sut = CreateSut();
        var actual = await sut.Handle(new SoftDeleteGroupCommand(group.Id), default);
        actual.DateDeleted.Should().NotBeNull();

        await _groupRepository.Received(1).UpdateAsync(Arg.Is<Group>(g => g.Id == group.Id && g.DateDeleted != null), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private SoftDeleteGroupCommandHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new SoftDeleteGroupCommandHandler(_unitOfWork, _logger);
    }
}