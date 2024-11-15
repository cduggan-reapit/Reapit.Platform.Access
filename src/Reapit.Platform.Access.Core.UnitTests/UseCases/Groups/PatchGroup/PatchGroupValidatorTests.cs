using Reapit.Platform.Access.Core.UnitTests.TestHelpers;
using Reapit.Platform.Access.Core.UseCases.Groups;
using Reapit.Platform.Access.Core.UseCases.Groups.PatchGroup;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.PatchGroup;

public class PatchGroupValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();

    [Fact]
    public async Task Validation_Succeeds_WhenNameAndDescriptionNull()
    {
        var command = GetRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validation_DoesNotFail_WhenGroupNotFound_AndValuesOtherwiseValid()
    {
        var command = GetRequest(name: "name", description: "description");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validation_Succeeds_WhenGroupFound_AndNameUnchanged()
    {
        var command = GetRequest(name: "same name", description: "new description");
        var group = new Group("same name", "old description", "organisation-id");
        
        // The group is found, the name is different, and it doesn't call GetGroups because it's unchanged.
        _groupRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(group);

        _groupRepository.GetGroupsAsync(
                cursor: null, 
                pageSize: 1, 
                userId: null, 
                organisationId: group.OrganisationId, 
                name: command.Name, 
                description: null, 
                createdFrom: null, 
                createdTo: null, 
                modifiedFrom: null, 
                modifiedTo: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new Group("other group", "which would cause failure", "if it sent this")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();

        await _groupRepository.DidNotReceive().GetGroupsAsync(
            Arg.Any<long?>(), 
            Arg.Any<int>(), 
            Arg.Any<string?>(),
            Arg.Any<string?>(), 
            Arg.Any<string?>(), 
            Arg.Any<string?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validation_Succeeds_WhenGroupFound_AndValuesValid()
    {
        var command = GetRequest(name: "new name", description: "new description");
        var group = new Group("old name", "old description", "organisation-id");
        
        // The group is found, the name is different, and nothing else exists with the same name.
        _groupRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(group);

        _groupRepository.GetGroupsAsync(
                cursor: null, 
                pageSize: 1, 
                userId: null, 
                organisationId: group.OrganisationId, 
                name: command.Name, 
                description: null, 
                createdFrom: null, 
                createdTo: null, 
                modifiedFrom: null, 
                modifiedTo: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Group>());
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validation_Fails_WhenNameTooLong()
    {
        var command = GetRequest(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchGroupCommand.Name), GroupValidationMessages.NameExceedsMaximumLength);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenGroupFound_AndNameUnavailable()
    {
        var command = GetRequest(name: "new name", description: "new description");
        var group = new Group("old name", "old description", "organisation-id");
        
        // The group is found, the name is different, and something else exists with the same name.
        _groupRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(group);

        _groupRepository.GetGroupsAsync(
                cursor: null, 
                pageSize: 1, 
                userId: null, 
                organisationId: group.OrganisationId, 
                name: command.Name, 
                description: null, 
                createdFrom: null, 
                createdTo: null, 
                modifiedFrom: null, 
                modifiedTo: null,
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new Group("other group", "which will cause failure", "when returned")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchGroupCommand.Name), GroupValidationMessages.NameUnavailable);

    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validation_Fails_WhenDescriptionTooLong()
    {
        var command = GetRequest(description: new string('a', 1001));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchGroupCommand.Description), GroupValidationMessages.DescriptionExceedsMaximumLength);
    }
    
    /*
     * Private methods
     */

    private PatchGroupCommandValidator CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        return new PatchGroupCommandValidator(_unitOfWork);
    }

    private static PatchGroupCommand GetRequest(string id = "id", string? name = null, string? description = null)
        => new(id, name, description);
}