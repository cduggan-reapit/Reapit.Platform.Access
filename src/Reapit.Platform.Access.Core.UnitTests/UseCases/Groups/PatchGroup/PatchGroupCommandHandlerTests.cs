using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.PatchGroup;

public class PatchGroupCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IValidator<PatchGroupCommand> _validator = Substitute.For<IValidator<PatchGroupCommand>>();
    private readonly FakeLogger<PatchGroupCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        ConfigureValidation(false);

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowNotFoundException_WhenGroupNotFound()
    {
        const string id = "test-id";
        
        ConfigureValidation(true);

        _groupRepository.GetGroupByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Group?>(null));

        var request = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsWithoutUpdate_WhenEntityUnchanged()
    {
        var group = new Group("name", "description", "organisation-id");
        
        ConfigureValidation(true);

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>())
            .Returns(group);

        var request = GetRequest(id: group.Id, name: group.Name, description: group.Description);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(group);

        await _groupRepository.DidNotReceive().UpdateAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_ReturnsAfterUpdate_WhenChanged()
    {
        var group = new Group("name", "description", "organisation-id");
        
        ConfigureValidation(true);

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>())
            .Returns(group);

        var request = GetRequest(id: group.Id, name: "new name", description: null);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(group);

        await _groupRepository.Received(1).UpdateAsync(Arg.Is<Group>(g => g.Name == request.Name), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchGroupCommandHandler CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new PatchGroupCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static PatchGroupCommand GetRequest(string id = "id", string? name = "name", string? description = "description")
        => new(id, name, description);

    private void ConfigureValidation(bool isSuccessful)
    {
        var result =  isSuccessful
            ? new ValidationResult()
            : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]);

        _validator.ValidateAsync(Arg.Any<PatchGroupCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);
    }
}