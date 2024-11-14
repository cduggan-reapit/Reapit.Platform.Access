
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.GroupMembership.AddGroupMember;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Identifiers;
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
                OrganisationUsers = [ 
                    new OrganisationUser { OrganisationId = "different-organisation-1"},
                    new OrganisationUser { OrganisationId = "different-organisation-2"},
                    new OrganisationUser { OrganisationId = "different-organisation-3"}
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
                OrganisationUsers = [ 
                    new OrganisationUser { OrganisationId = "different-organisation-1"},
                    new OrganisationUser { OrganisationId = "same-organisation"},
                    new OrganisationUser { OrganisationId = "different-organisation-2"}
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

    private static Group GetEntity(int groupId = 1, string? userId = "user-id")
    {
        using var _ = new GuidProviderContext(new Guid($"{groupId:D32}"));
        var group = new Group("name", "description", "organisationId");
        
        if(userId != null)
            group.AddUser(new User(userId, "name", "email"));

        return group;
    }
}