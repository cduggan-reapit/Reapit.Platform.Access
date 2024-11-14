using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Organisations.UpdateOrganisation;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.UpdateOrganisation;

public class UpdateOrganisationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<UpdateOrganisationCommand> _validator = Substitute.For<IValidator<UpdateOrganisationCommand>>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<UpdateOrganisationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<UpdateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenOrganisationDoesNotExist()
    {
        _validator.ValidateAsync(Arg.Any<UpdateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _organisationRepository.GetOrganisationByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsOrganisation_WhenOrganisationUpdated()
    {
        var organisation = new Organisation("id", "name");
        
        _validator.ValidateAsync(Arg.Any<UpdateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _organisationRepository.GetOrganisationByIdAsync(organisation.Id, Arg.Any<CancellationToken>())
            .Returns(organisation);

        var request = GetRequest(organisation.Id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(organisation);

        await _organisationRepository.Received(1)
            .UpdateOrganisationAsync(organisation, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private UpdateOrganisationCommandHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new UpdateOrganisationCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static UpdateOrganisationCommand GetRequest(string id = "default-id", string name = "default-name")
        => new(Id: id, Name: name);
}