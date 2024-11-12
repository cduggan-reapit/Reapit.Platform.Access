using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Organisations.CreateOrganisation;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;
using Reapit.Platform.Common.Providers.Temporal;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Organisations.CreateOrganisation;

public class CreateOrganisationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IValidator<CreateOrganisationCommand> _validator = Substitute.For<IValidator<CreateOrganisationCommand>>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly FakeLogger<CreateOrganisationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<CreateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsConflictException_WhenIdAlreadyExists()
    {
        const string id = "new-org-id";
        
        _validator.ValidateAsync(Arg.Any<CreateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _organisationRepository.GetOrganisationByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new Organisation("any", "any"));
        
        var request = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ConflictException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsOrganisation_WhenOrganisationCreated()
    {
        const string id = "new-org-id", name = "new-org-name";
        var timeFixture = new DateTimeOffset(2024, 11, 12, 15, 10, 11, TimeSpan.Zero);
        
        _validator.ValidateAsync(Arg.Any<CreateOrganisationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _organisationRepository.GetOrganisationByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));

        using var _ = new DateTimeOffsetProviderContext(timeFixture);
        var request = GetRequest(id, name);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(name);
        actual.DateLastSynchronised.Should().Be(timeFixture);
        await _organisationRepository.Received(1).CreateOrganisationAsync(actual, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private CreateOrganisationCommandHandler CreateSut()
    {
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new CreateOrganisationCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static CreateOrganisationCommand GetRequest(string id = "default-id", string name = "default-name")
        => new(Id: id, Name: name);
}