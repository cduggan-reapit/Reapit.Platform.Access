using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Roles.V1;
using Reapit.Platform.Access.Api.Controllers.Roles.V1.Models;
using Reapit.Platform.Access.Api.Controllers.Shared;
using Reapit.Platform.Access.Core.UseCases.UserRoles.AddUserRole;
using Reapit.Platform.Access.Core.UseCases.UserRoles.RemoveUserRole;
using Reapit.Platform.Access.Core.UseCases.Roles.CreateRole;
using Reapit.Platform.Access.Core.UseCases.Roles.DeleteRole;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoleById;
using Reapit.Platform.Access.Core.UseCases.Roles.GetRoles;
using Reapit.Platform.Access.Core.UseCases.Roles.PatchRole;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Roles.V1;

// These tests really just check that we're creating our queries/commands correctly
public class RolesControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<RolesProfile>())
        .CreateMapper();
    
    /*
     * GetRoles
     */

    [Fact]
    public async Task GetRoles_ReturnsOk_WithResultPage()
    {
        // The actual mapping is handled by the role profile and tested in RolesProfileTests.cs
        // We just want to make sure that it gets sent to mediator here.
        var requestModel = new GetRolesRequestModel(100);
        var query = _mapper.Map<GetRolesQuery>(requestModel);

        var roles = new[] { new Role("name", "description") };
        var expected = _mapper.Map<ResultPage<RoleModel>>(roles);
        
        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(roles);

        var sut = CreateSut();
        var response = await sut.GetRoles(requestModel) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ResultPage<RoleModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetRoleById
     */

    [Fact]
    public async Task GetRoleById_ReturnsOk_WithRoleModel()
    {
        const string id = "role-id";
        var role = new Role("name", "description");
        var expected = _mapper.Map<RoleModel>(role);
        
        _mediator.Send(Arg.Is<GetRoleByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>())
            .Returns(role);
        
        var sut = CreateSut();
        var response = await sut.GetRoleById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as RoleModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateRole
     */

    [Fact]
    public async Task CreateRole_ReturnsCreated_WithRoleModel()
    {
        var request = new CreateRoleRequestModel("name", "description");
        var command = new CreateRoleCommand(request.Name, request.Description);
        
        var role = new Role(request.Name, request.Description);
        var expected = _mapper.Map<RoleModel>(role);
        
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(role);
        
        var sut = CreateSut();
        var response = await sut.CreateRole(request) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match((CreatedAtActionResult result) => result.StatusCode == 201);

        var content = response!.Value as RoleModel;
        content.Should().BeEquivalentTo(expected);

        response.ActionName.Should().Be(nameof(RolesController.GetRoleById));
        response.RouteValues.Should().Contain(item => item.Key == "id" && role.Id.Equals(item.Value as string));
    }
    
    /*
     * PatchRole
     */
    
    [Fact]
    public async Task PatchRole_ReturnsNoContent()
    {
        var role = new Role("initial-name", "initial-description");
        var request = new PatchRoleRequestModel("new name", "new description");
        var command = new PatchRoleCommand(role.Id, request.Name, request.Description);
        
        var sut = CreateSut();
        var response = await sut.PatchRole(role.Id, request) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteRole
     */
    
    [Fact]
    public async Task DeleteRole_ReturnsNoContent()
    {
        var role = new Role("initial-name", "initial-description");
        var command = new SoftDeleteRoleCommand(role.Id);
        
        var sut = CreateSut();
        var response = await sut.DeleteRole(role.Id) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * AddUser
     */

    [Fact]
    public async Task AddMember_ReturnsNoContent()
    {
        const string roleId = "role-id", userId = "user-id";
        var command = new AddUserRoleCommand(roleId, userId);
        
        var sut = CreateSut();
        var response = await sut.AddUser(roleId, userId) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * RemoveMember
     */

    [Fact]
    public async Task RemoveMember_ReturnsNoContent()
    {
        const string roleId = "role-id", userId = "user-id";
        var command = new RemoveUserRoleCommand(roleId, userId);
        
        var sut = CreateSut();
        var response = await sut.RemoveUser(roleId, userId) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private RolesController CreateSut()
        => new(_mapper, _mediator);
}