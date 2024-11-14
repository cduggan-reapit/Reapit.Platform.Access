using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Groups.V1;
using Reapit.Platform.Access.Api.Controllers.Groups.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.UseCases.Groups.CreateGroup;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroupById;
using Reapit.Platform.Access.Core.UseCases.Groups.GetGroups;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Groups;

// These tests really just check that we're creating our queries/commands correctly
public class GroupsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<GroupsProfile>())
        .CreateMapper();
    
    /*
     * GetGroups
     */

    [Fact]
    public async Task GetGroups_ReturnsOk_WithResultPage()
    {
        // The actual mapping is handled by the group profile and tested in GroupsProfileTests.cs
        // We just want to make sure that it gets sent to mediator here.
        var requestModel = new GetGroupsRequestModel(100);
        var query = _mapper.Map<GetGroupsQuery>(requestModel);

        var groups = new[] { new Group("name", "description", "organisationid") };
        var expected = _mapper.Map<ResultPage<GroupModel>>(groups);
        
        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(groups);

        var sut = CreateSut();
        var response = await sut.GetGroups(requestModel) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ResultPage<GroupModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetGroupById
     */

    [Fact]
    public async Task GetGroupById_ReturnsOk_WithGroupModel()
    {
        const string id = "group-id";
        var group = new Group("name", "description", "organisationId");
        var expected = _mapper.Map<GroupModel>(group);
        
        _mediator.Send(Arg.Is<GetGroupByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>())
            .Returns(group);
        
        var sut = CreateSut();
        var response = await sut.GetGroupById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as GroupModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateGroup
     */

    [Fact]
    public async Task CreateGroup_ReturnsCreated_WithGroupModel()
    {
        var request = new CreateGroupRequestModel("name", "description", "organisationId");
        var command = new CreateGroupCommand(request.Name, request.Description, request.OrganisationId);
        
        var group = new Group(request.Name, request.Description, request.OrganisationId);
        var expected = _mapper.Map<GroupModel>(group);
        
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(group);
        
        var sut = CreateSut();
        var response = await sut.CreateGroup(request) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match((CreatedAtActionResult result) => result.StatusCode == 201);

        var content = response!.Value as GroupModel;
        content.Should().BeEquivalentTo(expected);

        response.ActionName.Should().Be(nameof(GroupsController.GetGroupById));
        response.RouteValues.Should().Contain(item => item.Key == "id" && group.Id.Equals(item.Value as string));
    }
    
    /*
     * Private methods
     */

    private GroupsController CreateSut()
        => new(_mapper, _mediator);
}