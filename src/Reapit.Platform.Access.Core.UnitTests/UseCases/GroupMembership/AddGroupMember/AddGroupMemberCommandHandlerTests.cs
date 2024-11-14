
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.GroupMembership.AddGroupMember;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.GroupMembership.AddGroupMember;

public class AddGroupMemberCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly FakeLogger<AddGroupMemberCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenGroupNotFound()
    {
        _groupRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Group?>(null));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsConflict_WhenUserAlreadyLinked()
    {
        var request = GetRequest();
        var group = new Group("name", "description", "organisation");
        group.AddUser(new User(request.UserId, "name", "email"));
        
        _groupRepository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(group);
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserNotFound()
    {
        var request = GetRequest();
        var group = new Group("name", "description", "organisation");
        
        _groupRepository.GetByIdAsync(request.GroupId, Arg.Any<CancellationToken>())
            .Returns(group);

        _userRepository.GetUserByIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsGroupMembershipException_WhenUserNotInGroupOrganisation()
    {
        var request = GetRequest();
        var group = new Group("name", "description", "same-organisation");
        
        _groupRepository.GetByIdAsync(request.GroupId, Arg.Any<CancellationToken>())
            .Returns(group);

        _userRepository.GetUserByIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(new User(request.UserId, "name", "email")
            {
                Organisations = [
                    new Organisation("different-organisation-1", string.Empty),
                    new Organisation("different-organisation-2", string.Empty),
                    new Organisation("different-organisation-3", string.Empty)
                ]
            });
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<GroupMembershipException>();
    }

    [Fact]
    public async Task Handle_AddsUserToGroup_WhenRequestValid()
    {
        var request = GetRequest();
        var group = new Group("name", "description", "same-organisation");
        
        _groupRepository.GetByIdAsync(request.GroupId, Arg.Any<CancellationToken>())
            .Returns(group);

        _userRepository.GetUserByIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(new User(request.UserId, "name", "email")
            {
                Organisations = [
                    new Organisation("different-organisation-1", string.Empty),
                    new Organisation("same-organisation", string.Empty),
                    new Organisation("different-organisation-3", string.Empty)
                ]
            });
        
        var sut = CreateSut();
        await sut.Handle(request, default);
        
        await _groupRepository.Received(1).UpdateAsync(Arg.Is<Group>(g => g.Id == group.Id && group.Users.Count == 1), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private AddGroupMemberCommandHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        _unitOfWork.Users.Returns(_userRepository);
        return new AddGroupMemberCommandHandler(_unitOfWork, _logger);
    }

    private static AddGroupMemberCommand GetRequest(int groupId = 1, string userId = "user-id")
        => new(new Guid($"{groupId:D32}").ToString("N"), userId);
}