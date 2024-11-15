using Reapit.Platform.Access.Core.UnitTests.TestHelpers;
using Reapit.Platform.Access.Core.UseCases;
using Reapit.Platform.Access.Core.UseCases.Groups;
using Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;
using Reapit.Platform.Access.Data.Repositories.Groups;
using Reapit.Platform.Access.Data.Repositories.Organisations;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Groups.CreateGroup;

public class CreateGroupCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IOrganisationRepository _organisationRepository = Substitute.For<IOrganisationRepository>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();

    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest();

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(request.OrganisationId, "name"));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }

    /*
     * Name
     */
    
    [Fact] 
    public async Task Validate_Fails_WhenNameIsEmpty()
    {
        var request = GetRequest(name: string.Empty);

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(request.OrganisationId, "name"));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGroupCommand.Name), CommonValidationMessages.Required);
    }
    
    [Fact] 
    public async Task Validate_Fails_WhenNameExceedsMaximumLength()
    {
        var request = GetRequest(name: new string('a', 101));

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(request.OrganisationId, "name"));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGroupCommand.Name), GroupValidationMessages.NameExceedsMaximumLength);
    }
    
    [Fact] 
    public async Task Validate_Fails_WhenNameAlreadyTaken()
    {
        var request = GetRequest(name: "name");

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(request.OrganisationId, "name"));
        
        _groupRepository.GetGroupsAsync(null, 1, null, request.OrganisationId, request.Name, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns([new Group("name", "description", "organisationId")]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGroupCommand.Name), GroupValidationMessages.NameUnavailable);
    }
    
    /*
     * Description
     */
    
    [Fact] 
    public async Task Validate_Fails_WhenDescriptionExceedsMaximumLength()
    {
        var request = GetRequest(description: new string('a', 1001));

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(new Organisation(request.OrganisationId, "name"));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGroupCommand.Description), GroupValidationMessages.DescriptionExceedsMaximumLength);
    }
    
    /*
     * OrganisationId
     */
    
    [Fact] 
    public async Task Validate_Fails_WhenOrganisationIdInvalid()
    {
        var request = GetRequest();

        _organisationRepository.GetOrganisationByIdAsync(request.OrganisationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Organisation?>(null));
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGroupCommand.OrganisationId), GroupValidationMessages.OrganisationNotFound);
    }

    /*
     * Private methods
     */

    private CreateGroupCommandValidator CreateSut()
    {
        _unitOfWork.Groups.Returns(_groupRepository);
        _unitOfWork.Organisations.Returns(_organisationRepository);
        return new CreateGroupCommandValidator(_unitOfWork);
    }

    private static CreateGroupCommand GetRequest(
        string name = "name", 
        string description = "",
        string organisationId = "organisationId")
        => new(name, description, organisationId);
}