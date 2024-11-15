using Reapit.Platform.Access.Core.UseCases.GroupMembership.RemoveGroupMember;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.GroupMembership.RemoveGroupMember;

public class RemoveGroupMemberCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly FakeLogger<RemoveGroupMemberCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task? Handle_ThrowsNotFound_WhenGroupNotFound()
    {
        const int groupNumber = 7;
        var request = GetRequest(groupNumber);
        
        _groupRepository.GetGroupByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Group?>(null));

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task? Handle_ThrowsNotFound_WhenUserNotInGroup()
    {
        const int groupNumber = 7;
        var request = GetRequest(groupNumber);
        var group = GetEntity(groupNumber, null);
        
        _groupRepository.GetGroupByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(group);

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task? Handle_RemovesUser_WhenUserInGroup()
    {
        const int groupNumber = 7;
        var request = GetRequest(groupNumber);
        var group = GetEntity(groupNumber, request.UserId);
        
        _groupRepository.GetGroupByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(group);

        var sut = CreateSut();
        await sut.Handle(request, default);

        await _groupRepository.Received(1).UpdateAsync(Arg.Is<Group>(g => g.Id == group.Id && !group.Users.Any()), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private RemoveGroupMemberCommandHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new RemoveGroupMemberCommandHandler(_unitOfWork, _logger);
    }

    private static RemoveGroupMemberCommand GetRequest(int groupId = 1, string userId = "user-id")
        => new(new Guid($"{groupId:D32}").ToString("N"), userId);

    private static Group GetEntity(int groupId = 1, string? userId = "user-id")
    {
        using var _ = new GuidProviderContext(new Guid($"{groupId:D32}"));
        var group = new Group("name", "description", "organisationId");
        
        if(userId != null)
            group.AddUser(new User(userId, "name", "email"));

        return group;
    }
}