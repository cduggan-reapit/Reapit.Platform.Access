using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Organisations.AddOrganisationMember;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Repositories.Users;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.AddOrganisationMember;

public class AddOrganisationMemberCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<AddOrganisationMemberCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenOrganisationDoesNotExist()
    {
        const string organisationId = "orgId", userId = "userId";
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsConflict_WhenUserAlreadyAssociatedWithOrganisation()
    {
        const string organisationId = "orgId", userId = "userId";
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(organisationId, "name")
            {
                OrganisationUsers = [
                    new OrganisationUser { UserId = userId}
                ]
            });

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        const string organisationId = "orgId", userId = "userId";
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(organisationId, organisationId));

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsRelationship_WhenCreated()
    {
        const string organisationId = "orgId", userId = "userId";

        var organisation = new Organisation(organisationId, "name");
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(organisation);

        var user = new User(userId, "name", "email");
        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.OrganisationId.Should().Be(organisationId);
        actual.UserId.Should().Be(userId);

        await _organisationRepository.Received(1).AddMemberAsync(actual, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private AddOrganisationMemberCommandHandler CreateSut()
    {
        _unitOfWork.Users.Returns(_userRepository);
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new AddOrganisationMemberCommandHandler(_unitOfWork, _logger);
    }

    private static AddOrganisationMemberCommand GetRequest(string organisationId = "organisation-id", string userId = "user-id")
        => new(organisationId, userId);
}