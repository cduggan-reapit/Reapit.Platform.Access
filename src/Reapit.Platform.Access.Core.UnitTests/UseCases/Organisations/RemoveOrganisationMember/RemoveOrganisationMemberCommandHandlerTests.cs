using Reapit.Platform.Access.Core.UseCases.Organisations.RemoveOrganisationMember;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Exceptions;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.RemoveOrganisationMember;

public class RemoveOrganisationMemberCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<RemoveOrganisationMemberCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenOrganisationNotFound()
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
    public async Task Handle_ThrowsNotFound_WhenRelationshipNotFoundInOrganisation()
    {
        const string organisationId = "orgId", userId = "userId";

        var organisation = new Organisation(organisationId, "name")
        {
            OrganisationUsers = new List<OrganisationUser>()
        };
        
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(organisation);

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsRelationship_WhenDeleted()
    {
        const string organisationId = "orgId", userId = "userId";

        var relationship = new OrganisationUser { OrganisationId = organisationId, UserId = userId };
        var organisation = new Organisation(organisationId, "name")
        {
            OrganisationUsers = [relationship]
        };
        
        _organisationRepository.GetOrganisationByIdAsync(organisationId, Arg.Any<CancellationToken>())
            .Returns(organisation);

        var request = GetRequest(organisationId, userId);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(relationship);

        await _organisationRepository.Received(1).RemoveMemberAsync(relationship, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private RemoveOrganisationMemberCommandHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new RemoveOrganisationMemberCommandHandler(_unitOfWork, _logger);
    }

    private static RemoveOrganisationMemberCommand GetRequest(string organisationId = "organisation-id", string userId = "user-id")
        => new(organisationId, userId);
}