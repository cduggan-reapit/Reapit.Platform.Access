using Reapit.Platform.Access.Core.UseCases.Organisations.DeleteOrganisation;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.DeleteOrganisation;

public class DeleteOrganisationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<DeleteOrganisationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenOrganisationDoesNotExist()
    {
        const string id = "test-id";
        _organisationRepository.GetOrganisationByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsDeletedOrganisation_WhenOrganisationFound()
    {
        const string id = "test-id";
        var expected = new Organisation(id, "random");
        _organisationRepository.GetOrganisationByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(expected);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);
        
        await _organisationRepository.Received(1)
            .DeleteOrganisationAsync(actual, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private DeleteOrganisationCommandHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new DeleteOrganisationCommandHandler(_unitOfWork, _logger);
    }

    private static DeleteOrganisationCommand GetRequest(string id = "default-id")
        => new(Id: id);
}