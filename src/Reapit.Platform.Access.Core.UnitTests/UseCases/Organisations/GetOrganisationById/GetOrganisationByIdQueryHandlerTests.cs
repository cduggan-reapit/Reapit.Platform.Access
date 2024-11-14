using Reapit.Platform.Access.Core.UseCases.Organisations.GetOrganisationById;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.GetOrganisationById;

public class GetOrganisationByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<GetOrganisationByIdQueryHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async void Handle_ThrowsNotFound_WhenUserNotFound()
    {
        _organisationRepository.GetOrganisationByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));
        
        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async void Handle_ReturnsEntity_WhenOrganisationFound()
    {
        const string id = "organisation-id";
        var expected = new Organisation(id, "arbitrary-name");
        _organisationRepository.GetOrganisationByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(expected);
        
        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private GetOrganisationByIdQueryHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new GetOrganisationByIdQueryHandler(_unitOfWork, _logger);
    }

    private static GetOrganisationByIdQuery GetRequest(string id = "default-id")
        => new(Id: id);
}