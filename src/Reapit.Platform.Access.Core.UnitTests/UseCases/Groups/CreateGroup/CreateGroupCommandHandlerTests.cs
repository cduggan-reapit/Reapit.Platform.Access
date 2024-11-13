using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.CreateGroup;

public class CreateGroupCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IValidator<CreateGroupCommand> _validator = Substitute.For<IValidator<CreateGroupCommand>>();
    private readonly FakeLogger<CreateGroupCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<CreateGroupCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsCreatedGroup_WhenOperationSuccessful()
    {
        _validator.ValidateAsync(Arg.Any<CreateGroupCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        var request = GetRequest();
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);

        actual.Name.Should().Be(request.Name);
        actual.Description.Should().Be(request.Description);
        actual.OrganisationId.Should().Be(request.OrganisationId);

        await _groupRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var group = new Group(request.Name, request.Description, request.OrganisationId);
        _ = await _unitOfWork.Groups.CreateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Group created: {id} ({blob})", group.Id, group.AsSerializable());
        return group;*/
    
    /*
     * Private methods
     */

    private CreateGroupCommandHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new CreateGroupCommandHandler(_unitOfWork, _validator, _logger);
    }
    
    private static CreateGroupCommand GetRequest(
        string name = "name", 
        string description = "",
        string organisationId = "organisationId")
        => new(name, description, organisationId);
}