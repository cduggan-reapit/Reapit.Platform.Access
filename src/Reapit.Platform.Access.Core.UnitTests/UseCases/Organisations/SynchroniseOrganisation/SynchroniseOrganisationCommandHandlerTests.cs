using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Organisations.SynchroniseOrganisation;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.SynchroniseOrganisation;

public class SynchroniseOrganisationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<SynchroniseOrganisationCommand> _validator = Substitute.For<IValidator<SynchroniseOrganisationCommand>>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<SynchroniseOrganisationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<SynchroniseOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_CreatesOrganisation_WhenOrganisationDoesNotExist()
    {
        _validator.ValidateAsync(Arg.Any<SynchroniseOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _organisationRepository.GetOrganisationByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));

        var request = GetRequest();
        var sut = CreateSut();
        _ = await sut.Handle(request, default);
        
        await _organisationRepository.Received(1).CreateOrganisationAsync(Arg.Is<Organisation>(o => o.Id == request.Id && o.Name == request.Name), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UpdatesOrganisation_WhenOrganisationAlreadyExists()
    {
        var organisation = new Organisation("id", "name");
        
        _validator.ValidateAsync(Arg.Any<SynchroniseOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _organisationRepository.GetOrganisationByIdAsync(organisation.Id, Arg.Any<CancellationToken>())
            .Returns(organisation);

        var request = GetRequest(organisation.Id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(organisation);

        await _organisationRepository.Received(1).UpdateOrganisationAsync(organisation, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private SynchroniseOrganisationCommandHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new SynchroniseOrganisationCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static SynchroniseOrganisationCommand GetRequest(string id = "default-id", string name = "default-name")
        => new(Id: id, Name: name);
}