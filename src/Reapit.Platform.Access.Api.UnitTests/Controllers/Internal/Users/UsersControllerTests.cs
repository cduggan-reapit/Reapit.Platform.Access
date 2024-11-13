using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1;
using Reapit.Platform.Access.Api.Controllers.Internal.Users.V1.Models;
using Reapit.Platform.Access.Core.UseCases.Users.CreateUser;
using Reapit.Platform.Access.Core.UseCases.Users.DeleteUser;
using Reapit.Platform.Access.Core.UseCases.Users.GetUserById;
using Reapit.Platform.Access.Core.UseCases.Users.UpdateUser;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Api.UnitTests.Controllers.Internal.Users;

// Most tests for controllers will be integration tests, this is _really_ just to check that we're mapping responses and
// building mediator request objects correctly.
public class UsersControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile(typeof(UsersProfile)))
        .CreateMapper();

    /*
     * GetUserById
     */

    [Fact]
    public async Task GetUserById_ReturnsOk_WithSimpleUserModel()
    {
        const string id = "id";
        var user = new User(id, "name", "email");
        var expected = _mapper.Map<SimpleUserModel>(user);

        _mediator.Send(Arg.Is<GetUserByIdQuery>(query => query.Id == id), Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.GetUserById(id) as OkObjectResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(200);
        actual.Value.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateUser
     */

    [Fact]
    public async Task CreateUser_ReturnsCreated_WithSimpleUserModel()
    {
        const string id = "id", name = "name", email = "email";
        var user = new User(id, name, email);
        var expected = _mapper.Map<SimpleUserModel>(user);
        
        _mediator.Send(
                request: Arg.Is<CreateUserCommand>(command => command.Id == id && command.Name == name && command.Email == email), 
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.CreateUser(new CreateUserRequestModel(id, name, email)) as CreatedAtActionResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(201);
        actual.Value.Should().BeEquivalentTo(expected);
        
        // Check the routing
        actual.ActionName.Should().Be(nameof(UsersController.GetUserById));
        actual.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string> { { "id", id } });
    }
    
    /*
     * UpdateUser
     */
    
    [Fact]
    public async Task UpdateUser_ReturnsNoContent()
    {
        const string id = "id", name = "name", email = "email";
        var user = new User(id, name, email);
        
        _mediator.Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.UpdateUser(id, new UpdateUserRequestModel(name, email)) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);
        
        await _mediator.Received(1).Send(
            request: Arg.Is<UpdateUserCommand>(command => command.Id == id && command.Name == name && command.Email == email),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteUser
     */
    
    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        const string id = "id";
        var user = new User(id, "name", "email");
        
        _mediator.Send(Arg.Any<DeleteUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var sut = CreateSut();
        var actual = await sut.DeleteUser(id) as NoContentResult;
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(204);

        await _mediator.Received(1).Send(
            request: Arg.Is<DeleteUserCommand>(command => command.Id == id),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private UsersController CreateSut()
        => new(_mapper, _mediator);
}